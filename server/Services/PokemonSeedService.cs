using Microsoft.EntityFrameworkCore;
using server.Data;

namespace server.Services
{
    public class PokemonSeedService
    {
        private readonly ApplicationDbContext _context;
        private readonly PokeApiService _pokeApiService;
        private readonly ILogger<PokemonSeedService> _logger;

        public PokemonSeedService(
            ApplicationDbContext context,
            PokeApiService pokeApiService,
            ILogger<PokemonSeedService> logger)
        {
            _context = context;
            _pokeApiService = pokeApiService;
            _logger = logger;
        }

        public async Task SeedPokemonSpeciesAsync(int startId = 1, int endId = 151)
        {
            _logger.LogInformation("Starting Pokemon species seed process from {StartId} to {EndId}", startId, endId);

            for (int i = startId; i <= endId; i++)
            {
                try
                {
                    // 既に存在する場合はスキップ
                    if (await _context.PokemonSpecies.AnyAsync(p => p.PokemonSpeciesId == i))
                    {
                        _logger.LogInformation("Pokemon species {Id} already exists, skipping", i);
                        continue;
                    }

                    _logger.LogInformation("Fetching Pokemon species {Id}...", i);
                    var species = await _pokeApiService.GetPokemonSpeciesAsync(i);

                    if (species != null)
                    {
                        await _context.PokemonSpecies.AddAsync(species);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Successfully saved Pokemon species {Id}: {Name}", 
                            species.PokemonSpeciesId, species.Name);
                    }

                    // API制限を考慮して少し待機
                    await Task.Delay(500);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error seeding Pokemon species {Id}", i);
                }
            }

            _logger.LogInformation("Pokemon species seed process completed");
        }

        public async Task SeedMovesAsync(int startId = 1, int endId = 165)
        {
            _logger.LogInformation("Starting Moves seed process from {StartId} to {EndId}", startId, endId);

            for (int i = startId; i <= endId; i++)
            {
                try
                {
                    // 既に存在する場合はスキップ
                    if (await _context.Moves.AnyAsync(m => m.MoveId == i))
                    {
                        _logger.LogInformation("Move {Id} already exists, skipping", i);
                        continue;
                    }

                    _logger.LogInformation("Fetching Move {Id}...", i);
                    var move = await _pokeApiService.GetMoveAsync(i);

                    if (move != null)
                    {
                        await _context.Moves.AddAsync(move);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Successfully saved Move {Id}: {Name}", 
                            move.MoveId, move.Name);
                    }

                    // API制限を考慮して少し待機
                    await Task.Delay(500);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error seeding Move {Id}", i);
                }
            }

            _logger.LogInformation("Moves seed process completed");
        }
    }
}