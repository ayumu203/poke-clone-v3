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
    // --- 新規追加: API レスポンスを JSON としてダンプ ---
    public async Task DumpPokemonJsonAsync(int startId = 1, int endId = 151, string folderPath = "Data/pokemons")
        {
            _logger.LogInformation("Dumping Pokemon JSON from {Start} to {End} into {Folder}", startId, endId, folderPath);
            for (int i = startId; i <= endId; i++)
            {
                try
                {
                    await _pokeApiService.SavePokemonJsonAsync(i, folderPath);
                    await Task.Delay(200);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error dumping pokemon json {Id}", i);
                }
            }
            _logger.LogInformation("Pokemon dump completed");
        }

    public async Task DumpMovesJsonAsync(int startId = 1, int endId = 165, string folderPath = "Data/moves")
        {
            _logger.LogInformation("Dumping Moves JSON from {Start} to {End} into {Folder}", startId, endId, folderPath);
            for (int i = startId; i <= endId; i++)
            {
                try
                {
                    await _pokeApiService.SaveMoveJsonAsync(i, folderPath);
                    await Task.Delay(200);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error dumping move json {Id}", i);
                }
            }
            _logger.LogInformation("Moves dump completed");
        }

        // --- 新規追加: 保存済 JSON から DB へ一括登録 ---
    public async Task SeedPokemonFromJsonFolderAsync(string folderPath = "Data/pokemons")
        {
            _logger.LogInformation("Seeding Pokemon from json folder {Folder}", folderPath);
            if (!Directory.Exists(folderPath))
            {
                _logger.LogWarning("Folder {Folder} does not exist", folderPath);
                return;
            }

            var files = Directory.GetFiles(folderPath, "pokemon_*.json").OrderBy(f => f).ToList();
            foreach (var file in files)
            {
                try
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var parts = fileName.Split('_');
                    if (parts.Length < 2) continue;
                    if (!int.TryParse(parts[1], out var id)) continue;

                    if (await _context.PokemonSpecies.AnyAsync(p => p.PokemonSpeciesId == id))
                    {
                        _logger.LogInformation("Pokemon species {Id} already exists, skipping", id);
                        continue;
                    }

                    var species = _pokeApiService.ParsePokemonSpeciesFromSavedJson(file, id);
                    if (species != null)
                    {
                        await _context.PokemonSpecies.AddAsync(species);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Saved species from json {Id} {Name}", id, species.Name);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error seeding species from file {File}", file);
                }
            }

            _logger.LogInformation("Seeding Pokemon from json completed");
        }

    public async Task SeedMovesFromJsonFolderAsync(string folderPath = "Data/moves")
        {
            _logger.LogInformation("Seeding Moves from json folder {Folder}", folderPath);
            if (!Directory.Exists(folderPath))
            {
                _logger.LogWarning("Folder {Folder} does not exist", folderPath);
                return;
            }

            var files = Directory.GetFiles(folderPath, "move_*.json").OrderBy(f => f).ToList();
            foreach (var file in files)
            {
                try
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var parts = fileName.Split('_');
                    if (parts.Length < 2) continue;
                    if (!int.TryParse(parts[1], out var id)) continue;

                    if (await _context.Moves.AnyAsync(m => m.MoveId == id))
                    {
                        _logger.LogInformation("Move {Id} already exists, skipping", id);
                        continue;
                    }

                    var move = _pokeApiService.ParseMoveFromSavedJson(file, id);
                    if (move != null)
                    {
                        await _context.Moves.AddAsync(move);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Saved move from json {Id} {Name}", id, move.Name);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error seeding move from file {File}", file);
                }
            }

            _logger.LogInformation("Seeding Moves from json completed");
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