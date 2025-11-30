using Microsoft.EntityFrameworkCore;
using Server.Domain.Entities;
using Server.Domain.Repositories;
using Server.Infrastructure.Data;

namespace Server.Infrastructure.Repositories;

public class PokemonRepository : IPokemonRepository
{
    private readonly AppDbContext _context;

    public PokemonRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Pokemon?> GetByIdAsync(string pokemonId)
    {
        return await _context.Pokemons
            .Include(p => p.Species)
            .Include(p => p.Moves)
            .FirstOrDefaultAsync(p => p.PokemonId == pokemonId);
    }

    public async Task<List<Pokemon>> GetPlayerPartyAsync(string playerId)
    {
        var playerParty = await _context.PlayerParties
            .AsSplitQuery()
            .Include(pp => pp.Party)
                .ThenInclude(p => p.Species)
            .Include(pp => pp.Party)
                .ThenInclude(p => p.Moves)
            .FirstOrDefaultAsync(pp => pp.PlayerId == playerId);

        return playerParty?.Party ?? new List<Pokemon>();
    }

    public async Task AddToPartyAsync(string playerId, Pokemon pokemon)
    {
        await _context.Pokemons.AddAsync(pokemon);
        
        var playerParty = await _context.PlayerParties
            .Include(pp => pp.Party)
            .FirstOrDefaultAsync(pp => pp.PlayerId == playerId);

        if (playerParty == null)
        {
            // PlayerPartyが存在しない場合は新規作成
            playerParty = new PlayerParty
            {
                PlayerId = playerId,
                Party = new List<Pokemon> { pokemon }
            };
            await _context.PlayerParties.AddAsync(playerParty);
        }
        else
        {
            playerParty.Party.Add(pokemon);
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Pokemon pokemon)
    {
        _context.Pokemons.Update(pokemon);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsPartyFullAsync(string playerId)
    {
        var playerParty = await _context.PlayerParties
            .Include(pp => pp.Party)
            .FirstOrDefaultAsync(pp => pp.PlayerId == playerId);

        return playerParty != null && playerParty.Party.Count >= 6;
    }
}
