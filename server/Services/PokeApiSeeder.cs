using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models.Basics;
using System.Linq;

namespace server.Services
{
    public class PokeApiSeeder
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PokeApiSeeder> _logger;

        public PokeApiSeeder(HttpClient httpClient, ApplicationDbContext context, ILogger<PokeApiSeeder> logger)
        {
            _httpClient = httpClient;
            _context = context;
            _logger = logger;
        }

        public async Task ExtractPokemonSpeciesAsync(int startId, int endId)
        {
            _logger.LogInformation("PokeAPI よりポケモンデータの取得を開始します.");
            var seedDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Seed", "Pokemon");
            Directory.CreateDirectory(seedDirectory);

            for (int id = startId; id <= endId; id++)
            {
                var filePath = Path.Combine(seedDirectory, $"{id}.json");
                if (File.Exists(filePath))
                {
                    _logger.LogInformation($"ポケモン ID {id} のファイルは既に存在するため、ダウンロードをスキップします.");
                    continue;
                }

                try
                {
                    var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{id}");
                    response.EnsureSuccessStatusCode();
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    await File.WriteAllTextAsync(filePath, jsonContent);
                    _logger.LogInformation($"ポケモン ID {id} のデータを正常にダウンロードし、保存しました.");
                    await Task.Delay(200);
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError(e, $"ポケモン ID {id} のデータのダウンロードに失敗しました.");
                }
            }
            _logger.LogInformation("ポケモンデータの取得が完了しました.");
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
                    _logger.LogInformation($"ポケモン種族 ID {id} はデータベースに既に存在するため、スキップします。");
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
                _logger.LogInformation($"ロード準備完了: '{species.Name}' (ID: {id})");
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
            
            // 既存の関連データをメモリにロードして重複チェックを高速化
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

            _logger.LogWarning($"Stat '{statName}' not found for a Pokemon. Defaulting to 0.");
            return 0;
        }
    }
}

