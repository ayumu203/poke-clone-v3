using Server.Domain.Entities;

namespace Server.Domain.Repositories;

public interface IPokemonSpeciesRepository
{
    Task<PokemonSpecies?> GetByIdAsync(int pokemonSpeciesId);
    Task<List<PokemonSpecies>> GetAllAsync();
    Task AddAsync(PokemonSpecies species);
    Task UpdateAsync(PokemonSpecies species);
}
