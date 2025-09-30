using Microsoft.AspNetCore.Mvc;
using server.Services;
using System.Threading.Tasks;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedingController : ControllerBase
    {
        private readonly PokeApiSeeder _seeder;
        private readonly IWebHostEnvironment _env;

        public SeedingController(PokeApiSeeder seeder, IWebHostEnvironment env)
        {
            _seeder = seeder;
            _env = env;
        }
        [HttpPost("seed-species")]
        public async Task<IActionResult> SeedPokemonSpecies()
        {
            if (!_env.IsDevelopment())
            {
                return Forbid("このエンドポイントは開発環境でのみ有効です.");
            }
            await _seeder.ExtractPokemonSpeciesAsync(1, 151);
            // await _seeder.LoadPokemonSpeciesToDbAsync();
            return Ok("データの取得は無事終了しました.");
        }
    }
}