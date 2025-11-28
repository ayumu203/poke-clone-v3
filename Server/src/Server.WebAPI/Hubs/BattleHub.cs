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
                
                // 結果をグループ全体にブロードキャスト
                await Clients.Group(battleId).SendAsync("ReceiveTurnResult", result);

                if (result.IsBattleEnd)
                {
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
