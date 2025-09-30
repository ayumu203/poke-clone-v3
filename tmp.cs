using System.Text.Json;
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
            _logger.LogInformation("Starting PokemonSpecies data extraction from PokeAPI...");
            var seedDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Seed", "PokemonSpecies");
            Directory.CreateDirectory(seedDirectory);

            for (int id = startId; id <= endId; id++)
            {
                var filePath = Path.Combine(seedDirectory, $"{id}.json");
                if (File.Exists(filePath))
                {
                    _logger.LogInformation($"Skipping download for PokemonSpecies ID {id}, file already exists.");
                    continue;
                }

                try
                {
                    var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon-species/{id}");
                    response.EnsureSuccessStatusCode();
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    await File.WriteAllTextAsync(filePath, jsonContent);
                    _logger.LogInformation($"Successfully downloaded and saved PokemonSpecies ID {id}.");
                    await Task.Delay(200);
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError(e, $"Failed to download data for PokemonSpecies ID {id}.");
                }
            }
            _logger.LogInformation("PokemonSpecies data extraction finished.");
        }

        public async Task LoadPokemonSpeciesToDbAsync()
        {
            _logger.LogInformation("Loading PokemonSpecies from local files to database...");
            var seedDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Seed", "PokemonSpecies");
            if (!Directory.Exists(seedDirectory))
            {
                _logger.LogWarning("Seed directory for PokemonSpecies does not exist. Nothing to load.");
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
                    _logger.LogInformation($"Skipping PokemonSpecies ID {id}, already exists in database.");
                    continue;
                }

                var species = new PokemonSpecies
                {
                    PokemonSpeciesId = id,
                    Name = root.GetProperty("names").EnumerateArray()
                        .FirstOrDefault(n => n.GetProperty("language").GetProperty("name").GetString() == "ja-Hrkt")
                        .GetProperty("name").GetString() ?? "N/A",
                    BaseHp = 0,
                    BaseAttack = 0,
                    BaseDefence = 0,
                    BaseSpecialAttack = 0,
                    BaseSpecialDefence = 0,
                    BaseSpeed = 0,
                    Type1 = "N/A",
                    FrontImage = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{id}.png",
                    BackImage = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/back/{id}.png"
                };
                newSpeciesList.Add(species);
                _logger.LogInformation($"Prepared PokemonSpecies '{species.Name}' (ID: {id}) for insertion.");
            }

            if (newSpeciesList.Any())
            {
                _context.PokemonSpecies.AddRange(newSpeciesList);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully loaded {newSpeciesList.Count} new PokemonSpecies into the database.");
            }
            else
            {
                _logger.LogInformation("No new PokemonSpecies to load.");
            }
        }
    }
}
