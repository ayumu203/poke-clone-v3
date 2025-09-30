using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using server.Interfaces;

namespace server.Services
{
    public class PokeApiExtractor : IPokeApiExtractor
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PokeApiExtractor> _logger;

        public PokeApiExtractor(HttpClient httpClient, ILogger<PokeApiExtractor> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task ExtractPokemonSpeciesAsync(int startId, int endId)
        {
            _logger.LogInformation("PokeAPI よりポケモンデータの取得を開始します.");
            var seedDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Seed", "Pokemon");
            Directory.CreateDirectory(seedDirectory);

            for (int id = startId; id <= endId; id++)
            {
                await DownloadAndSaveAsync($"https://pokeapi.co/api/v2/pokemon/{id}", Path.Combine(seedDirectory, $"{id}.json"), $"ポケモン ID {id}");
                await Task.Delay(200);
            }
            _logger.LogInformation("ポケモンデータの取得が完了しました.");
        }

        public async Task ExtractMovesAsync(int startId, int endId)
        {
            _logger.LogInformation("PokeAPI よりわざデータの取得を開始します.");
            var seedDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Seed", "Moves");
            Directory.CreateDirectory(seedDirectory);

            for (int id = startId; id <= endId; id++)
            {
                await DownloadAndSaveAsync($"https://pokeapi.co/api/v2/move/{id}", Path.Combine(seedDirectory, $"{id}.json"), $"わざ ID {id}");
                await Task.Delay(200);
            }
            _logger.LogInformation("わざデータの取得が完了しました.");
        }

        private async Task DownloadAndSaveAsync(string requestUri, string filePath, string logIdentifier)
        {
            if (File.Exists(filePath))
            {
                _logger.LogInformation($"{logIdentifier} のファイルは既に存在するため、ダウンロードをスキップします.");
                return;
            }
            try
            {
                var response = await _httpClient.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();
                var jsonContent = await response.Content.ReadAsStringAsync();
                await File.WriteAllTextAsync(filePath, jsonContent);
                _logger.LogInformation($"{logIdentifier} のデータを正常にダウンロードし、保存しました.");
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"{logIdentifier} のデータのダウンロードに失敗しました.");
            }
        }
    }
}
