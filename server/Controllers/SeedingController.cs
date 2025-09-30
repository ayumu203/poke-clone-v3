using Microsoft.AspNetCore.Mvc;
using server.Interfaces;
using System.Threading.Tasks;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedingController : ControllerBase
    {
        private readonly IPokeApiExtractor _extractor;
        private readonly IDatabaseSeeder _seeder;
        private readonly IWebHostEnvironment _env;

        public SeedingController(IPokeApiExtractor extractor, IDatabaseSeeder seeder, IWebHostEnvironment env)
        {
            _extractor = extractor;
            _seeder = seeder;
            _env = env;
        }

        [HttpPost("extract-all")]
        public async Task<IActionResult> ExtractAllData()
        {
            if (!_env.IsDevelopment()) return Forbid("このエンドポイントは開発環境でのみ有効です.");

            await _extractor.ExtractPokemonSpeciesAsync(1, 151);
            await _extractor.ExtractMovesAsync(1, 919);

            return Ok("すべてのデータの抽出とファイル保存が完了しました。");
        }

        [HttpPost("load-all")]
        public async Task<IActionResult> LoadAllDataToDb()
        {
            if (!_env.IsDevelopment()) return Forbid("このエンドポイントは開発環境でのみ有効です.");

            // 依存関係の順序でロードを実行
            await _seeder.LoadMovesToDbAsync();
            await _seeder.LoadPokemonSpeciesToDbAsync();
            await _seeder.LoadPokemonMovesToDbAsync();
            
            return Ok("すべてのデータのデータベースへのロードが完了しました。");
        }
    }
}

