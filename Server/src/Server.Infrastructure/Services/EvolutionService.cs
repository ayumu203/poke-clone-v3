using Microsoft.EntityFrameworkCore;
using Server.Domain.Entities;
using Server.Domain.Repositories;
using Server.Domain.Services;
using Server.Infrastructure.Data;

namespace Server.Infrastructure.Services;

public class EvolutionService : IEvolutionService
{
    private readonly AppDbContext _context;
    private readonly IPokemonSpeciesRepository _pokemonSpeciesRepository;

    public EvolutionService(
        AppDbContext context,
        IPokemonSpeciesRepository pokemonSpeciesRepository)
    {
        _context = context;
        _pokemonSpeciesRepository = pokemonSpeciesRepository;
    }

    public async Task<bool> CanEvolveAsync(int pokemonSpeciesId, int currentLevel)
    {
        var species = await _pokemonSpeciesRepository.GetByIdAsync(pokemonSpeciesId);
        
        if (species == null || species.EvolveLevel == 0)
        {
            return false;
        }

        return currentLevel >= species.EvolveLevel;
    }

    public async Task<PokemonSpecies?> GetEvolutionAsync(int pokemonSpeciesId, int currentLevel)
    {
        var canEvolve = await CanEvolveAsync(pokemonSpeciesId, currentLevel);
        
        if (!canEvolve)
        {
            return null;
        }

        // 進化先のSpeciesIdを取得
        // 注: PokemonSpeciesに進化先IDフィールドがないため、簡易実装として
        // SpeciesId + 1 を進化先とする（フシギダネ=1 → フシギソウ=2の想定）
        // 実際の実装では EvolvesToSpeciesId フィールドを追加するか、
        // 別途進化マッピングテーブルを用意する必要がある
        var evolutionSpeciesId = pokemonSpeciesId + 1;
        
        return await _pokemonSpeciesRepository.GetByIdAsync(evolutionSpeciesId);
    }
}
