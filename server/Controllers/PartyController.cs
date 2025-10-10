using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Helpers;
using server.Models.Core;
using server.Models.DTOs;

namespace server.Controllers;

[ApiController]
[Route("api/players/{playerId}/party")]
[Authorize] 
public class PartyController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PartyController> _logger;

    public PartyController(
        ApplicationDbContext context,
        ILogger<PartyController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// プレイヤーの手持ちポケモンを取得
    /// </summary>
    /// <param name="playerId">プレイヤーID</param>
    /// <returns>手持ちポケモンリスト</returns>
    [HttpGet]
    public async Task<ActionResult<PlayerPartyListDto>> GetPlayerParty(string playerId)
    {
        try
        {
            if (!JwtHelper.IsAuthorized(User, playerId))
            {
                _logger.LogWarning("Unauthorized access attempt to party for player {PlayerId}", playerId);
                return Forbid();
            }

            var player = await _context.Players
                .AsNoTracking()
                .Include(p => p.Pokemons)
                    .ThenInclude(pokemon => pokemon.Species)
                .Include(p => p.Pokemons)
                    .ThenInclude(pokemon => pokemon.LearnedMoves)
                        .ThenInclude(lm => lm.Move)
                .FirstOrDefaultAsync(p => p.PlayerId == playerId);

            if (player == null)
            {
                return NotFound(new { message = $"Player with id {playerId} not found" });
            }

            var partyList = new PlayerPartyListDto
            {
                Pokemons = player.Pokemons.Select(p => MapToPokemonDto(p)).ToList()
            };

            return Ok(partyList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting player party {PlayerId}", playerId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// 手持ちポケモンを追加
    /// </summary>
    /// <param name="playerId">プレイヤーID</param>
    /// <param name="dto">追加するポケモン情報</param>
    /// <returns>追加されたポケモン情報</returns>
    [HttpPost]
    public async Task<ActionResult<PokemonDto>> AddPokemonToParty(
        string playerId,
        [FromBody] CreatePokemonDto dto)
    {
        try
        {
            // 認可チェック: 自分のデータのみ追加可能
            if (!JwtHelper.IsAuthorized(User, playerId))
            {
                _logger.LogWarning("Unauthorized add attempt to party for player {PlayerId}", playerId);
                return Forbid();
            }

            var player = await _context.Players
                .Include(p => p.Pokemons)
                .FirstOrDefaultAsync(p => p.PlayerId == playerId);

            if (player == null)
            {
                return NotFound(new { message = $"Player with id {playerId} not found" });
            }

            // 手持ちポケモンの上限チェック（6匹まで）
            if (player.Pokemons.Count >= 6)
            {
                return BadRequest(new { message = "Party is full. Maximum 6 pokemon allowed." });
            }

            // ポケモン種族が存在するか確認
            var species = await _context.PokemonSpecies
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.PokemonSpeciesId == dto.PokemonSpeciesId);

            if (species == null)
            {
                return NotFound(new { message = $"Pokemon species with id {dto.PokemonSpeciesId} not found" });
            }

            // ステータス計算
            var maxHp = CalculateStat(species.BaseHP, dto.Level, isHP: true);
            var attack = CalculateStat(species.BaseAttack, dto.Level);
            var defense = CalculateStat(species.BaseDefense, dto.Level);
            var specialAttack = CalculateStat(species.BaseSpecialAttack, dto.Level);
            var specialDefense = CalculateStat(species.BaseSpecialDefense, dto.Level);
            var speed = CalculateStat(species.BaseSpeed, dto.Level);

            var pokemon = new Pokemon
            {
                PlayerId = playerId,
                PokemonSpeciesId = dto.PokemonSpeciesId,
                Level = dto.Level,
                Exp = 0,
                CurrentHP = maxHp,
                Rank = new Rank(),
                Ailment = Models.Enums.Ailment.None
            };

            _context.Pokemons.Add(pokemon);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Pokemon added to party: {PokemonId} for player {PlayerId}", 
                pokemon.PokemonId, playerId);

            // レスポンス用にロード
            await _context.Entry(pokemon)
                .Reference(p => p.Species)
                .LoadAsync();

            await _context.Entry(pokemon)
                .Collection(p => p.LearnedMoves)
                .LoadAsync();

            var result = MapToPokemonDto(pokemon);

            return CreatedAtAction(
                nameof(GetPlayerParty),
                new { playerId = playerId },
                result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding pokemon to party {PlayerId}", playerId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// 手持ちポケモンを更新
    /// </summary>
    /// <param name="playerId">プレイヤーID</param>
    /// <param name="dto">更新するポケモン情報</param>
    /// <returns>更新後のポケモン情報</returns>
    [HttpPut]
    public async Task<ActionResult<PokemonDto>> UpdatePokemonInParty(
        string playerId,
        [FromBody] UpdatePokemonDto dto)
    {
        try
        {
            // 認可チェック: 自分のデータのみ更新可能
            if (!JwtHelper.IsAuthorized(User, playerId))
            {
                _logger.LogWarning("Unauthorized update attempt to party for player {PlayerId}", playerId);
                return Forbid();
            }

            var pokemon = await _context.Pokemons
                .Include(p => p.Species)
                .Include(p => p.LearnedMoves)
                    .ThenInclude(lm => lm.Move)
                .FirstOrDefaultAsync(p => p.PokemonId == dto.PokemonId && p.PlayerId == playerId);

            if (pokemon == null)
            {
                return NotFound(new { message = $"Pokemon with id {dto.PokemonId} not found" });
            }

            // 更新可能なフィールドを更新
            // Nicknameはポケモンモデルに存在しないため削除

            if (dto.CurrentHP.HasValue)
            {
                // MaxHPは計算で求める必要があります
                var maxHp = CalculateStat(pokemon.Species.BaseHP, pokemon.Level, isHP: true);
                pokemon.CurrentHP = Math.Max(0, Math.Min(dto.CurrentHP.Value, maxHp));
            }

            if (dto.Experience.HasValue)
            {
                pokemon.Exp = dto.Experience.Value;
            }

            if (dto.Ailment.HasValue)
            {
                pokemon.Ailment = dto.Ailment.Value;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Pokemon updated: {PokemonId} for player {PlayerId}", 
                pokemon.PokemonId, playerId);

            var result = MapToPokemonDto(pokemon);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating pokemon {PokemonId}", dto.PokemonId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// 手持ちポケモンを削除
    /// </summary>
    /// <param name="playerId">プレイヤーID</param>
    /// <param name="pokemonId">ポケモンID</param>
    /// <returns>削除結果</returns>
    [HttpDelete("{pokemonId}")]
    public async Task<ActionResult> RemovePokemonFromParty(string playerId, int pokemonId)
    {
        try
        {
            // 認可チェック: 自分のデータのみ削除可能
            if (!JwtHelper.IsAuthorized(User, playerId))
            {
                _logger.LogWarning("Unauthorized delete attempt to party for player {PlayerId}", playerId);
                return Forbid();
            }

            var pokemon = await _context.Pokemons
                .FirstOrDefaultAsync(p => p.PokemonId == pokemonId && p.PlayerId == playerId);

            if (pokemon == null)
            {
                return NotFound(new { message = $"Pokemon with id {pokemonId} not found" });
            }

            _context.Pokemons.Remove(pokemon);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Pokemon deleted: {PokemonId} for player {PlayerId}", 
                pokemonId, playerId);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing pokemon {PokemonId}", pokemonId);
            return StatusCode(500, "Internal server error");
        }
    }

    // ヘルパーメソッド

    /// <summary>
    /// ステータス値を計算
    /// </summary>
    private int CalculateStat(int baseStat, int level, bool isHP = false)
    {
        if (isHP)
        {
            return ((baseStat * 2) * level / 100) + level + 10;
        }
        else
        {
            return ((baseStat * 2) * level / 100) + 5;
        }
    }

    /// <summary>
    /// PokemonエンティティをPokemonDtoにマッピング
    /// </summary>
    private PokemonDto MapToPokemonDto(Pokemon pokemon)
    {
        if (pokemon.Species == null)
        {
            throw new InvalidOperationException("Pokemon species is not loaded");
        }

        return new PokemonDto
        {
            PokemonId = pokemon.PokemonId,
            Nickname = pokemon.Species.Name,
            Species = new PokemonSpeciesDto
            {
                PokemonSpeciesId = pokemon.Species.PokemonSpeciesId,
                Name = pokemon.Species.Name,
                FrontImage = pokemon.Species.FrontImage,
                BackImage = pokemon.Species.BackImage,
                Type1 = pokemon.Species.Type1.ToString(),
                Type2 = pokemon.Species.Type2?.ToString(),
                EvolveLevel = pokemon.Species.EvolveLevel,
                BaseHp = pokemon.Species.BaseHP,
                BaseAttack = pokemon.Species.BaseAttack,
                BaseDefence = pokemon.Species.BaseDefense,
                BaseSpecialAttack = pokemon.Species.BaseSpecialAttack,
                BaseSpecialDefence = pokemon.Species.BaseSpecialDefense,
                BaseSpeed = pokemon.Species.BaseSpeed,
                MoveList = new List<MoveDto>()
            },
            Level = pokemon.Level,
            Exp = pokemon.Exp,
            CurrentHp = pokemon.CurrentHP,
            LearnedMoves = pokemon.LearnedMoves.ToList(),
            Rank = pokemon.Rank,
            Ailment = pokemon.Ailment.ToString(),
            VolatileStatus = new VolatileStatus()
        };
    }
}