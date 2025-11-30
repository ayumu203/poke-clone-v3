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
        lock (_pendingActions)
        {
            if (!_pendingActions.ContainsKey(battleId))
            {
                _pendingActions[battleId] = new List<PlayerAction>();
            }
            
            _pendingActions[battleId].Add(action);
        }

        // 両プレイヤーのアクションが揃ったらターン処理
        if (_pendingActions[battleId].Count == 2)
        {
            var actions = _pendingActions[battleId].ToList();
            _pendingActions.Remove(battleId);

            try
            {
                var result = await _battleService.ProcessTurnAsync(battleId, actions[0], actions[1]);
                
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
