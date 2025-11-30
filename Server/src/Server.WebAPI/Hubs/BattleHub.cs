using Microsoft.AspNetCore.SignalR;
using Server.Application.Services;
using Server.Domain.Entities;

namespace Server.WebAPI.Hubs;

public class BattleHub : Hub
{
    private readonly IBattleService _battleService;
    private static readonly Dictionary<string, List<PlayerAction>> _pendingActions = new();

    public BattleHub(IBattleService battleService)
    {
        _battleService = battleService;
    }

    public async Task JoinBattle(string battleId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, battleId);
        
        var battleState = await _battleService.GetBattleStateAsync(battleId);
        if (battleState != null)
        {
            await Clients.Caller.SendAsync("BattleStarted", battleState);
        }
    }

    public async Task SubmitAction(string battleId, PlayerAction action)
    {
        // 1. Add Player's Action
        lock (_pendingActions)
        {
            if (!_pendingActions.ContainsKey(battleId))
            {
                _pendingActions[battleId] = new List<PlayerAction>();
            }
            
            // Prevent duplicate actions from the same player
            if (!_pendingActions[battleId].Any(a => a.PlayerId == action.PlayerId))
            {
                _pendingActions[battleId].Add(action);
            }
        }

        // 2. Check for CPU Opponent and generate action if needed
        // Note: This assumes Player2 is always the CPU in a CPU battle
        var battleState = await _battleService.GetBattleStateAsync(battleId);
        if (battleState != null && battleState.Player2.PlayerId == "CPU" && action.PlayerId != "CPU")
        {
            // Generate CPU Action
            var cpuPlayer = battleState.Player2;
            var activePokemon = cpuPlayer.PokemonEntities[cpuPlayer.ActivePokemonIndex];
            var random = new Random();
            
            // Simple AI: Randomly select a move
            // Ensure we have moves
            int moveId = 0;
            if (activePokemon.Moves != null && activePokemon.Moves.Any())
            {
                var randomMove = activePokemon.Moves[random.Next(activePokemon.Moves.Count)];
                moveId = randomMove.MoveId;
            }

            var cpuAction = new PlayerAction
            {
                PlayerId = "CPU",
                ActionType = Server.Domain.Enums.ActionType.Attack,
                Value = moveId
            };

            lock (_pendingActions)
            {
                 if (!_pendingActions[battleId].Any(a => a.PlayerId == "CPU"))
                 {
                     _pendingActions[battleId].Add(cpuAction);
                 }
            }
        }

        // 3. Check if we have 2 actions to process
        List<PlayerAction>? actionsToProcess = null;
        lock (_pendingActions)
        {
            if (_pendingActions.ContainsKey(battleId) && _pendingActions[battleId].Count == 2)
            {
                actionsToProcess = _pendingActions[battleId].ToList();
                _pendingActions.Remove(battleId);
            }
        }

        // 4. Process Turn
        if (actionsToProcess != null)
        {
            try
            {
                var result = await _battleService.ProcessTurnAsync(battleId, actionsToProcess[0], actionsToProcess[1]);
                
                // ターン結果をグループ全体にブロードキャスト
                await Clients.Group(battleId).SendAsync("ReceiveTurnResult", result);

                // 捕獲成功の処理
                var catchResult = result.ActionResults
                    .FirstOrDefault(ar => ar.CatchResult?.IsSuccess == true);
                if (catchResult != null)
                {
                    await _battleService.ProcessPostBattleAsync(battleId, result);
                    await Clients.Group(battleId).SendAsync("BattleEnded", "Caught");
                    await _battleService.DeleteBattleAsync(battleId);
                    await Clients.Group(battleId).SendAsync("BattleClosed");
                    return;
                }

                // 逃走成功の処理
                var escapeResult = result.ActionResults
                    .FirstOrDefault(ar => ar.EscapeResult?.IsSuccess == true);
                if (escapeResult != null)
                {
                    await Clients.Group(battleId).SendAsync("BattleEnded", "Escaped");
                    await _battleService.DeleteBattleAsync(battleId);
                    await Clients.Group(battleId).SendAsync("BattleClosed");
                    return;
                }

                // 通常のバトル終了処理
                if (result.IsBattleEnd)
                {
                    // 経験値・進化処理
                    await _battleService.ProcessPostBattleAsync(battleId, result);
                    
                    await Clients.Group(battleId).SendAsync("BattleEnded", result.WinnerId);
                    await _battleService.DeleteBattleAsync(battleId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProcessTurn: {ex}");
                await Clients.Group(battleId).SendAsync("Error", ex.Message);
            }
        }
    }

    public async Task LeaveBattle(string battleId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, battleId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // クリーンアップ処理
        await base.OnDisconnectedAsync(exception);
    }
}
