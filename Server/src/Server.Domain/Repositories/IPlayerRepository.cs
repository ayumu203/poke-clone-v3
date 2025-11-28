using Server.Domain.Entities;

namespace Server.Domain.Repositories;

public interface IPlayerRepository
{
    Task<Player?> GetByIdAsync(string playerId);
    Task<List<Player>> GetAllAsync();
    Task AddAsync(Player player);
    Task UpdateAsync(Player player);
    Task DeleteAsync(string playerId);
}
