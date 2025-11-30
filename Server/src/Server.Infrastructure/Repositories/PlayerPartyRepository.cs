using Microsoft.EntityFrameworkCore;
using Server.Domain.Entities;
using Server.Domain.Repositories;
using Server.Infrastructure.Data;

namespace Server.Infrastructure.Repositories;

public class PlayerPartyRepository : IPlayerPartyRepository
{
    private readonly AppDbContext _context;

    public PlayerPartyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PlayerParty?> GetByPlayerIdAsync(string playerId)
    {
        return await _context.PlayerParties
            .Include(pp => pp.Player)
            .Include(pp => pp.Party)
                .ThenInclude(p => p.Species)
            .Include(pp => pp.Party)
                .ThenInclude(p => p.Moves)
            .FirstOrDefaultAsync(pp => pp.PlayerId == playerId);
    }

    public async Task AddPokemonToPartyAsync(string playerId, string pokemonId)
    {
        var playerParty = await _context.PlayerParties
            .Include(pp => pp.Party)
            .FirstOrDefaultAsync(pp => pp.PlayerId == playerId);

        if (playerParty == null)
        {
            throw new InvalidOperationException($"PlayerParty not found for player {playerId}");
        }

        // PlayerPartyPokemon中間テーブルへの追加はEF Coreが自動で行う
        // Pokemon エンティティが既に存在していることが前提
        var pokemon = await _context.Pokemons.FindAsync(pokemonId);
        if (pokemon == null)
        {
            throw new InvalidOperationException($"Pokemon not found: {pokemonId}");
        }

        playerParty.Party.Add(pokemon);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PlayerParty playerParty)
    {
        _context.PlayerParties.Update(playerParty);
        await _context.SaveChangesAsync();
    }
}
