using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models.Core;
using System.Linq;
using Microsoft.Extensions.Logging;
using server.Interfaces;

namespace server.Services
{
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(ApplicationDbContext context, ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LoadPokemonSpeciesToDbAsync()
        {
            _logger.LogInformation("ローカルファイルからデータベースへのポケモン種族データのロードを開始します.");
            var seedDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Seed", "Pokemon");
            if (!Directory.Exists(seedDirectory))
            {
                _logger.LogWarning("シードディレクトリが存在しません。ロードするデータがありません。");
                return;
            }

            var files = Directory.GetFiles(seedDirectory, "*.json");
            var newSpeciesList = new List<PokemonSpecies>();

            foreach (var file in files)
            {
                var jsonContent = await File.ReadAllTextAsync(file);
                using var doc = JsonDocument.Parse(jsonContent);
                var root = doc.RootElement;

                var id = root.GetProperty("id").GetInt32();
                if (await _context.PokemonSpecies.AnyAsync(ps => ps.PokemonSpeciesId == id))
                {
                    continue;
                }
                
                var species = new PokemonSpecies
                {
                    PokemonSpeciesId = id,
                    FrontImage = root.GetProperty("sprites").GetProperty("front_default").GetString() ?? "",
                    BackImage = root.GetProperty("sprites").GetProperty("back_default").GetString() ?? ""
                };
                
                var nameEn = root.GetProperty("name").GetString() ?? "N/A";
                species.Name = char.ToUpper(nameEn[0]) + nameEn.Substring(1);

                var stats = root.GetProperty("stats").EnumerateArray();
                species.BaseHp = GetStatValue(stats, "hp");
                species.BaseAttack = GetStatValue(stats, "attack");
                species.BaseDefense = GetStatValue(stats, "defense");
                species.BaseSpecialAttack = GetStatValue(stats, "special-attack");
                species.BaseSpecialDefense = GetStatValue(stats, "special-defense");
                species.BaseSpeed = GetStatValue(stats, "speed");

                var types = root.GetProperty("types").EnumerateArray().OrderBy(t => t.GetProperty("slot").GetInt32());
                species.Type1 = types.FirstOrDefault().GetProperty("type").GetProperty("name").GetString() ?? "N/A";
                species.Type2 = types.Count() > 1 ? types.LastOrDefault().GetProperty("type").GetProperty("name").GetString() : null;

                species.EvolveLevel = null;

                newSpeciesList.Add(species);
            }

            if (newSpeciesList.Any())
            {
                await _context.PokemonSpecies.AddRangeAsync(newSpeciesList);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"データベースに {newSpeciesList.Count} 件の新しいポケモン種族データを正常にロードしました。");
            }
            else
            {
                _logger.LogInformation("データベースに追加する新しいポケモン種族データはありませんでした。");
            }
        }

        public async Task LoadMovesToDbAsync()
        {
            _logger.LogInformation("ローカルファイルからデータベースへのわざデータのロードを開始します。");
            var seedDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Seed", "Moves");
            if (!Directory.Exists(seedDirectory))
            {
                _logger.LogWarning("わざのシードディレクトリが存在しません。ロードするデータがありません。");
                return;
            }

            var files = Directory.GetFiles(seedDirectory, "*.json");
            var newMoves = new List<Move>();

            foreach (var file in files)
            {
                var jsonContent = await File.ReadAllTextAsync(file);
                using var doc = JsonDocument.Parse(jsonContent);
                var root = doc.RootElement;

                var id = root.GetProperty("id").GetInt32();
                if (await _context.Moves.AnyAsync(m => m.MoveId == id))
                {
                    continue;
                }

                var move = new Move
                {
                    MoveId = id,
                    Name = root.GetProperty("names").EnumerateArray()
                        .FirstOrDefault(n => n.GetProperty("language").GetProperty("name").GetString() == "ja-Hrkt")
                        .GetProperty("name").GetString() ?? root.GetProperty("name").GetString() ?? "N/A",
                    Type = root.GetProperty("type").GetProperty("name").GetString() ?? "N/A",
                    DamageClass = root.GetProperty("damage_class").GetProperty("name").GetString() ?? "N/A",
                    Power = root.TryGetProperty("power", out var powerEl) && powerEl.ValueKind == JsonValueKind.Number ? powerEl.GetInt32() : 0,
                    Pp = root.TryGetProperty("pp", out var ppEl) && ppEl.ValueKind == JsonValueKind.Number ? ppEl.GetInt32() : 0,
                    Accuracy = root.TryGetProperty("accuracy", out var accEl) && accEl.ValueKind == JsonValueKind.Number ? accEl.GetInt32() : 0,
                    Priority = root.GetProperty("priority").GetInt32(),
                };
                newMoves.Add(move);
            }

            if (newMoves.Any())
            {
                await _context.Moves.AddRangeAsync(newMoves);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"データベースに {newMoves.Count} 件の新しいわざデータを正常にロードしました。");
            }
            else
            {
                _logger.LogInformation("データベースに追加する新しいわざデータはありませんでした。");
            }
        }
        
        public async Task LoadPokemonMovesToDbAsync()
        {
            _logger.LogInformation("ローカルファイルからデータベースへのポケモン-わざ関連データのロードを開始します。");
            var seedDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Seed", "Pokemon");
            if (!Directory.Exists(seedDirectory))
            {
                _logger.LogWarning("シードディレクトリが存在しません。ロードするデータがありません。");
                return;
            }

            var files = Directory.GetFiles(seedDirectory, "*.json");
            var newPokemonMoves = new List<PokemonMove>();
            
            var existingMoves = await _context.PokemonMoves.ToDictionaryAsync(pm => (pm.PokemonSpeciesId, pm.MoveId));

            foreach (var file in files)
            {
                var jsonContent = await File.ReadAllTextAsync(file);
                using var doc = JsonDocument.Parse(jsonContent);
                var root = doc.RootElement;

                var pokemonId = root.GetProperty("id").GetInt32();

                if (!root.TryGetProperty("moves", out var movesElement)) continue;

                foreach (var moveEntry in movesElement.EnumerateArray())
                {
                    var moveUrl = moveEntry.GetProperty("move").GetProperty("url").GetString();
                    if (string.IsNullOrEmpty(moveUrl)) continue;

                    var segments = moveUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    if (segments.Length > 0 && int.TryParse(segments.Last(), out var moveId))
                    {
                        if (existingMoves.ContainsKey((pokemonId, moveId)) || newPokemonMoves.Any(m => m.PokemonSpeciesId == pokemonId && m.MoveId == moveId))
                        {
                            continue;
                        }

                        var pokemonMove = new PokemonMove
                        {
                            PokemonSpeciesId = pokemonId,
                            MoveId = moveId
                        };
                        newPokemonMoves.Add(pokemonMove);
                    }
                }
            }

            if (newPokemonMoves.Any())
            {
                await _context.PokemonMoves.AddRangeAsync(newPokemonMoves);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"データベースに {newPokemonMoves.Count} 件の新しいポケモン-わざ関連データを正常にロードしました。");
            }
            else
            {
                _logger.LogInformation("データベースに追加する新しいポケモン-わざ関連データはありませんでした。");
            }
        }

        private int GetStatValue(JsonElement.ArrayEnumerator stats, string statName)
        {
            var statElement = stats.FirstOrDefault(s => s.GetProperty("stat").GetProperty("name").GetString() == statName);

            if (statElement.ValueKind != JsonValueKind.Undefined && statElement.TryGetProperty("base_stat", out var baseStatElement))
            {
                return baseStatElement.GetInt32();
            }

            return 0;
        }
    }
}
