using Microsoft.EntityFrameworkCore;
using Server.Domain.Entities;
using Server.Domain.Repositories;
using Server.Infrastructure.Data;

namespace Server.Infrastructure.Repositories;

public class MoveRepository : IMoveRepository
{
    private readonly AppDbContext _context;

    public MoveRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Move?> GetByIdAsync(int moveId)
    {
        return await _context.Moves.FindAsync(moveId);
    }

    public async Task<List<Move>> GetAllAsync()
    {
        return await _context.Moves.ToListAsync();
    }

    public async Task AddAsync(Move move)
    {
        await _context.Moves.AddAsync(move);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Move move)
    {
        _context.Moves.Update(move);
        await _context.SaveChangesAsync();
    }
}
