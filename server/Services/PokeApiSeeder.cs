using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using server.Data;
using server.Models;

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
            _logger.LogInformation("PokeAPI よりポケモン種族データの取得を開始します.");
            var seedDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Seed", "PokemonSpecies");
            Directory.CreateDirectory(seedDirectory);

            for (int id = startId; id <= endId; id++)
            {
                var filePath = Path.Combine(seedDirectory, $"{id}.json");
                try
                {
                    var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{id}");
                    response.EnsureSuccessStatusCode();
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    await File.WriteAllTextAsync(filePath, jsonContent);
                    _logger.LogInformation($"ポケモン種族 ID {id} のデータを正常にダウンロードし、保存しました.");
                    await Task.Delay(200);
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError(e, $"ポケモン種族 ID {id} のデータのダウンロードに失敗しました.");
                }
            }
            _logger.LogInformation("ポケモン種族データの取得が完了しました.");
        }
    }
}