using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Domain.Entities;
using Server.Domain.Repositories;

namespace Server.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GachaController : ControllerBase
{
    private readonly IPokemonSpeciesRepository _pokemonSpeciesRepository;
    private readonly IPokemonRepository _pokemonRepository;
    private readonly IPlayerRepository _playerRepository;
    private const int GachaCost = 5000;

    public GachaController(
        IPokemonSpeciesRepository pokemonSpeciesRepository,
        IPokemonRepository pokemonRepository,
        IPlayerRepository playerRepository)
    {
        _pokemonSpeciesRepository = pokemonSpeciesRepository;
        _pokemonRepository = pokemonRepository;
        _playerRepository = playerRepository;
    }

    /// <summary>
    /// ガチャを引く
    /// </summary>
    [HttpPost("pull")]
    public async Task<ActionResult<Pokemon>> PullGacha()
    {
        var playerId = User.Identity?.Name;
        if (string.IsNullOrEmpty(playerId))
        {
            return Unauthorized();
        }

        // プレイヤー情報を取得
        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
        {
            return NotFound("プレイヤーが見つかりません");
        }

        // 所持金が足りるか確認
        if (player.Money < GachaCost)
        {
            return BadRequest($"所持金が不足しています（必要: {GachaCost}円, 所持: {player.Money}円）");
        }

        // パーティが6体未満であることを確認
        var isPartyFull = await _pokemonRepository.IsPartyFullAsync(playerId);
        if (isPartyFull)
        {
            return BadRequest("パーティが満杯です");
        }

        // ランダムなポケモンを生成
        var allSpecies = await _pokemonSpeciesRepository.GetAllAsync();
        if (allSpecies == null || allSpecies.Count == 0)
        {
            return StatusCode(500, "ポケモンデータが見つかりません");
        }

        var random = new Random();
        var randomSpecies = allSpecies[random.Next(allSpecies.Count)];
        var randomLevel = random.Next(1, 11); // レベル1-10

        var pokemon = new Pokemon
        {
            PokemonId = Guid.NewGuid().ToString(),
            Species = randomSpecies,
            Level = randomLevel,
            Exp = 0,
            Moves = randomSpecies.MoveList.Take(4).ToList()
        };

        // 所持金を減算
        player.Money -= GachaCost;
        await _playerRepository.UpdateAsync(player);

        // パーティに追加
        await _pokemonRepository.AddToPartyAsync(playerId, pokemon);

        return Ok(new 
        { 
            message = "ガチャを引きました",
            pokemon,
            remainingMoney = player.Money
        });
    }
}
