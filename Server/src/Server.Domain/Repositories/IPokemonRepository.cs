using Server.Domain.Entities;

namespace Server.Domain.Repositories;

public interface IPokemonRepository
{
    Task<Pokemon?> GetByIdAsync(string pokemonId);
    Task<List<Pokemon>> GetPlayerPartyAsync(string playerId);
    Task AddToPartyAsync(string playerId, Pokemon pokemon);
    Task UpdateAsync(Pokemon pokemon);
    Task<bool> IsPartyFullAsync(string playerId);
    Task<int> GetPartyCountAsync(string playerId);
    Task RemoveFromPartyAsync(string playerId, string pokemonId);
    Task DeletePokemonAsync(string pokemonId);
}
