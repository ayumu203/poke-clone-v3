using Server.Domain.Entities;

namespace Server.Domain.Repositories;

public interface IMoveRepository
{
    Task<Move?> GetByIdAsync(int moveId);
    Task<List<Move>> GetAllAsync();
    Task AddAsync(Move move);
    Task UpdateAsync(Move move);
}
