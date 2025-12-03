using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Domain.Entities;
using Server.Domain.Repositories;
using System.Security.Claims;

namespace Server.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StarterController : ControllerBase
{
    private readonly IPokemonSpeciesRepository _pokemonSpeciesRepository;
    private readonly IPokemonRepository _pokemonRepository;
    private readonly IPlayerRepository _playerRepository;

    // Starter Pokemon: ヒコザル(390), ゼニガメ(7), ツタージャ(495)
    private static readonly int[] StarterSpeciesIds = { 390, 7, 495 };
    private const int StarterPokemonLevel = 5;

    public StarterController(
        IPokemonSpeciesRepository pokemonSpeciesRepository,
        IPokemonRepository pokemonRepository,
        IPlayerRepository playerRepository)
    {
        _pokemonSpeciesRepository = pokemonSpeciesRepository;
        _pokemonRepository = pokemonRepository;
        _playerRepository = playerRepository;
    }

    /// <summary>
    /// スターターポケモンの選択肢を取得
    /// </summary>
    [HttpGet("options")]
    public async Task<ActionResult<List<PokemonSpecies>>> GetStarterOptions()
    {
        var allSpecies = await _pokemonSpeciesRepository.GetAllAsync();
        var starters = allSpecies
            .Where(s => StarterSpeciesIds.Contains(s.PokemonSpeciesId))
            .ToList();

        if (starters.Count == 0)
        {
            return NotFound("スターターポケモンが見つかりません");
        }

        return Ok(starters);
    }

    /// <summary>
    /// スターターポケモンを選択
    /// </summary>
    [HttpPost("select")]
    public async Task<Microsoft.AspNetCore.Mvc.ActionResult> SelectStarter([FromBody] SelectStarterRequest request)
    {
        var playerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(playerId))
        {
            return Unauthorized();
        }

        // プレイヤーが既にポケモンを所持していないか確認
        var existingParty = await _pokemonRepository.GetPlayerPartyAsync(playerId);
        if (existingParty != null && existingParty.Count > 0)
        {
            return BadRequest("既にポケモンを所持しています");
        }

        // 選択されたSpeciesIdがスターターポケモンであることを確認
        if (!StarterSpeciesIds.Contains(request.PokemonSpeciesId))
        {
            return BadRequest("無効なスターターポケモンです");
        }

        // ポケモン種族データを取得
        var species = await _pokemonSpeciesRepository.GetByIdAsync(request.PokemonSpeciesId);
        if (species == null)
        {
            return NotFound("ポケモン種族が見つかりません");
        }

        // レベル5のポケモンを生成
        var pokemon = new Pokemon
        {
            PokemonId = Guid.NewGuid().ToString(),
            PokemonSpeciesId = species.PokemonSpeciesId,
            Level = StarterPokemonLevel,
            Exp = 0,
            Moves = species.MoveList.Take(4).ToList()
        };

        // プレイヤーのパーティに追加
        await _pokemonRepository.AddToPartyAsync(playerId, pokemon);

        return Ok(new { message = "スターターポケモンを選択しました", pokemon });
    }
}

public class SelectStarterRequest
{
    public int PokemonSpeciesId { get; set; }
}
