using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models.DTOs;

namespace server.Controllers;

[ApiController]
[Route("api/pokemon-species")]
public class PokemonSpeciesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PokemonSpeciesController> _logger;

    public PokemonSpeciesController(
        ApplicationDbContext context,
        ILogger<PokemonSpeciesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// ポケモン種族一覧を取得
    /// </summary>
    /// <param name="skip">スキップする件数</param>
    /// <param name="take">取得する件数</param>
    /// <returns>ポケモン種族一覧</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PokemonSpeciesDto>>> GetPokemonSpecies(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            var species = await _context.PokemonSpecies
                .AsNoTracking()
                .OrderBy(s => s.PokemonSpeciesId)
                .Skip(skip)
                .Take(take)
                .Select(s => new PokemonSpeciesDto
                {
                    PokemonSpeciesId = s.PokemonSpeciesId,
                    Name = s.Name,
                    FrontImage = s.FrontImage,
                    BackImage = s.BackImage,
                    Type1 = s.Type1.ToString(),
                    Type2 = s.Type2.HasValue ? s.Type2.Value.ToString() : null,
                    EvolveLevel = s.EvolveLevel,
                    BaseHp = s.BaseHP,
                    BaseAttack = s.BaseAttack,
                    BaseDefence = s.BaseDefense,
                    BaseSpecialAttack = s.BaseSpecialAttack,
                    BaseSpecialDefence = s.BaseSpecialDefense,
                    BaseSpeed = s.BaseSpeed,
                    MoveList = new List<MoveDto>() // 必要に応じて実装
                })
                .ToListAsync();

            return Ok(species);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pokemon species list");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// 特定のポケモン種族を取得
    /// </summary>
    /// <param name="id">ポケモン種族ID</param>
    /// <returns>ポケモン種族データ</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<PokemonSpeciesDto>> GetPokemonSpecies(int id)
    {
        try
        {
            var species = await _context.PokemonSpecies
                .AsNoTracking()
                .Where(s => s.PokemonSpeciesId == id)
                .Select(s => new PokemonSpeciesDto
                {
                    PokemonSpeciesId = s.PokemonSpeciesId,
                    Name = s.Name,
                    FrontImage = s.FrontImage,
                    BackImage = s.BackImage,
                    Type1 = s.Type1.ToString(),
                    Type2 = s.Type2.HasValue ? s.Type2.Value.ToString() : null,
                    EvolveLevel = s.EvolveLevel,
                    BaseHp = s.BaseHP,
                    BaseAttack = s.BaseAttack,
                    BaseDefence = s.BaseDefense,
                    BaseSpecialAttack = s.BaseSpecialAttack,
                    BaseSpecialDefence = s.BaseSpecialDefense,
                    BaseSpeed = s.BaseSpeed,
                    MoveList = new List<MoveDto>()
                })
                .FirstOrDefaultAsync();

            if (species == null)
            {
                return NotFound(new { message = $"Pokemon species with id {id} not found" });
            }

            return Ok(species);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pokemon species {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}