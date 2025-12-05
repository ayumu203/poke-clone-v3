using Server.Domain.Entities;

namespace Server.Domain.Repositories;

public interface IBattleRepository
{
    Task<BattleState?> GetAsync(string battleId);
    Task SaveAsync(BattleState battleState);
    Task<bool> TryLockAsync(string battleId, TimeSpan expiry);
    Task UnlockAsync(string battleId);
    Task DeleteAsync(string battleId);
}
