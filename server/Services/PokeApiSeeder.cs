using System.Text
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
            _logger.LogInformation("PokeAPI よりポケモン種族データの取得を開始します.")
        }
    }
}