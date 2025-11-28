using Server.Domain.Entities;

namespace Server.Application.Services;

public interface IBattleService
{
    Task<BattleState> CreateBattleAsync(string player1Id, string player2Id);
    Task<BattleState> CreateCpuBattleAsync(string playerId);
    Task<ProcessResult> ProcessTurnAsync(string battleId, PlayerAction action1, PlayerAction action2);
    Task<BattleState?> GetBattleStateAsync(string battleId);
    Task SaveBattleStateAsync(BattleState battleState);
    Task DeleteBattleAsync(string battleId);
}
