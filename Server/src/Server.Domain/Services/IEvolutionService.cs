using Server.Domain.Entities;

namespace Server.Domain.Services;

public interface IEvolutionService
{
    Task<PokemonSpecies?> GetEvolutionAsync(int pokemonSpeciesId, int currentLevel);
    Task<bool> CanEvolveAsync(int pokemonSpeciesId, int currentLevel);
}
