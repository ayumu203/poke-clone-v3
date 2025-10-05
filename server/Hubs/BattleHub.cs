using Microsoft.AspNetCore.SignalR;
using server.Models.Battles.Core;
using server.Models.Battles.Services;
using server.Models.Battles.Players;

namespace server.Hubs
{
    public class BattleHub : Hub
    {
        private readonly BattleRoomManager _battleManager;
        private readonly CpuPlayer _cpuPlayer;

        public BattleHub(BattleRoomManager battleManager, CpuPlayer cpuPlayer)
        {
            _battleManager = battleManager;
            _cpuPlayer = cpuPlayer;
        }

        public async Task JoinBattleRoom(string battleRoomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, battleRoomId);
        }

        public async Task SendAction(string battleRoomId, PlayerAction playerAction)
        {
            var battleRoom = _battleManager.GetBattleRoom(battleRoomId);
            if (battleRoom == null)
            {
                await Clients.Caller.SendAsync("Error", "指定された対戦ルームは存在しません。");
                return;
            }
            
            var cpuAction = _cpuPlayer.ChooseAction(battleRoom.Battle);
            
            var turnResults = battleRoom.Battle.ExecuteTurn(playerAction, cpuAction);

            await Clients.Group(battleRoomId).SendAsync("ReceiveTurnResult", turnResults);

            if (battleRoom.Battle.State == GameState.Finished)
            {
                // ToDo: BattleResultオブジェクトを定義して返す
                // await Clients.Group(battleRoomId).SendAsync("BattleFinished", battleResult);
                _battleManager.RemoveBattleRoom(battleRoomId);
            }
        }

        public override async Task OnDisconnectedAsync(System.Exception? exception)
        {
            // ToDo: クライアント切断時のルーム削除処理など
            await base.OnDisconnectedAsync(exception);
        }
    }
}