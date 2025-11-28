using Microsoft.EntityFrameworkCore;
using Server.Domain.Entities;
using Server.Domain.Repositories;
using Server.Infrastructure.Data;

namespace Server.Infrastructure.Repositories;

public class PokemonSpeciesRepository : IPokemonSpeciesRepository
{
    private readonly AppDbContext _context;

    public PokemonSpeciesRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PokemonSpecies?> GetByIdAsync(int pokemonSpeciesId)
    {
        return await _context.PokemonSpecies
            .Include(ps => ps.MoveList)
            .FirstOrDefaultAsync(ps => ps.PokemonSpeciesId == pokemonSpeciesId);
    }

    public async Task<List<PokemonSpecies>> GetAllAsync()
    {
        return await _context.PokemonSpecies
            .Include(ps => ps.MoveList)
            .ToListAsync();
    }

    public async Task AddAsync(PokemonSpecies species)
    {
        await _context.PokemonSpecies.AddAsync(species);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PokemonSpecies species)
    {
        _context.PokemonSpecies.Update(species);
        await _context.SaveChangesAsync();
    }
}
