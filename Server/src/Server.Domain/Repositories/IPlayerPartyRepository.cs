using Server.Domain.Entities;

namespace Server.Domain.Repositories;

public interface IPlayerPartyRepository
{
    Task<PlayerParty?> GetByPlayerIdAsync(string playerId);
    Task AddPokemonToPartyAsync(string playerId, string pokemonId);
    Task UpdateAsync(PlayerParty playerParty);
}
