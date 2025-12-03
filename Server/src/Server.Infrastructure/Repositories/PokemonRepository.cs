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
            .Include(pp => pp.Party)
                .ThenInclude(p => p.Species)
            .Include(pp => pp.Party)
                .ThenInclude(p => p.Moves)
            .FirstOrDefaultAsync(pp => pp.PlayerId == playerId);

        if (playerParty == null)
        {
            return new List<Pokemon>();
        }

        return playerParty.Party;
    }

    public async Task AddToPartyAsync(string playerId, Pokemon pokemon)
    {
        var playerParty = await _context.PlayerParties
            .Include(pp => pp.Party)
            .FirstOrDefaultAsync(pp => pp.PlayerId == playerId);

        if (playerParty == null)
        {
            // Playerが存在するか確認
            var playerExists = await _context.Players.AnyAsync(p => p.PlayerId == playerId);
            if (!playerExists)
            {
                throw new InvalidOperationException($"Player with ID '{playerId}' does not exist. Please create a player profile first.");
            }

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
            // PartyリストにPokemonを追加すると、EFが自動的にPokemonを追跡する
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

    public async Task<int> GetPartyCountAsync(string playerId)
    {
        var playerParty = await _context.PlayerParties
            .Include(pp => pp.Party)
            .FirstOrDefaultAsync(pp => pp.PlayerId == playerId);

        return playerParty?.Party.Count ?? 0;
    }

    public async Task RemoveFromPartyAsync(string playerId, string pokemonId)
    {
        var playerParty = await _context.PlayerParties
            .Include(pp => pp.Party)
            .FirstOrDefaultAsync(pp => pp.PlayerId == playerId);

        if (playerParty != null)
        {
            var pokemon = playerParty.Party.FirstOrDefault(p => p.PokemonId == pokemonId);
            if (pokemon != null)
            {
                playerParty.Party.Remove(pokemon);
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task DeletePokemonAsync(string pokemonId)
    {
        var pokemon = await _context.Pokemons.FindAsync(pokemonId);
        if (pokemon != null)
        {
            _context.Pokemons.Remove(pokemon);
            await _context.SaveChangesAsync();
        }
    }
}
