using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Domain.Entities;
using Server.Domain.Repositories;

namespace Server.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PartyController : ControllerBase
{
    private readonly IPokemonRepository _pokemonRepository;

    public PartyController(IPokemonRepository pokemonRepository)
    {
        _pokemonRepository = pokemonRepository;
    }

    /// <summary>
    /// プレイヤーのパーティ一覧を取得
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<Pokemon>>> GetParty()
    {
        var playerId = User.Identity?.Name;
        if (string.IsNullOrEmpty(playerId))
        {
            return StatusCode(500, "ユーザー情報の取得に失敗しました");
        }

        var party = await _pokemonRepository.GetPlayerPartyAsync(playerId);
        if (party == null || party.Count == 0)
        {
            return Ok(new List<Pokemon>());
        }

        return Ok(party);
    }

    /// <summary>
    /// ポケモンを逃がす
    /// </summary>
    [HttpDelete("{pokemonId}")]
    public async Task<Microsoft.AspNetCore.Mvc.ActionResult> ReleasePokemon(string pokemonId)
    {
        var playerId = User.Identity?.Name;
        if (string.IsNullOrEmpty(playerId))
        {
            return Unauthorized();
        }

        // 指定されたポケモンがプレイヤーのパーティに存在するか確認
        var party = await _pokemonRepository.GetPlayerPartyAsync(playerId);
        if (party.Count <= 1)
        {
            return BadRequest("最後のポケモンは逃がせません");
        }
        
        var pokemon = party?.FirstOrDefault(p => p.PokemonId == pokemonId);
        
        if (pokemon == null)
        {
            return NotFound("指定されたポケモンが見つかりません");
        }

        // パーティからポケモンを削除
        await _pokemonRepository.RemoveFromPartyAsync(playerId, pokemonId);
        await _pokemonRepository.DeletePokemonAsync(pokemonId);

        return Ok(new { message = "ポケモンを逃がしました" });
    }
}
