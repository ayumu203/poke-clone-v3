using System.Text.Json;
using StackExchange.Redis;
using Server.Domain.Entities;
using Server.Domain.Repositories;

namespace Server.Infrastructure.Repositories;

public class RedisBattleRepository : IBattleRepository
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private const string BattleKeyPrefix = "battle:";
    private const string LockKeyPrefix = "battle:lock:";

    public RedisBattleRepository(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = _redis.GetDatabase();
    }

    public async Task<BattleState?> GetAsync(string battleId)
    {
        var key = GetBattleKey(battleId);
        var value = await _db.StringGetAsync(key);
        
        if (value.IsNullOrEmpty)
        {
            return null;
        }

        return JsonSerializer.Deserialize<BattleState>(value!);
    }

    public async Task SaveAsync(BattleState battleState)
    {
        var key = GetBattleKey(battleState.BattleId);
        var json = JsonSerializer.Serialize(battleState);
        
        // バトルは1時間で自動削除
        await _db.StringSetAsync(key, json, TimeSpan.FromHours(1));
    }

    public async Task<bool> TryLockAsync(string battleId, TimeSpan expiry)
    {
        var lockKey = GetLockKey(battleId);
        // SETNX: キーが存在しない場合のみ設定
        return await _db.StringSetAsync(lockKey, "locked", expiry, When.NotExists);
    }

    public async Task UnlockAsync(string battleId)
    {
        var lockKey = GetLockKey(battleId);
        await _db.KeyDeleteAsync(lockKey);
    }

    public async Task DeleteAsync(string battleId)
    {
        var key = GetBattleKey(battleId);
        await _db.KeyDeleteAsync(key);
    }

    private static string GetBattleKey(string battleId) => $"{BattleKeyPrefix}{battleId}";
    private static string GetLockKey(string battleId) => $"{LockKeyPrefix}{battleId}";
}
