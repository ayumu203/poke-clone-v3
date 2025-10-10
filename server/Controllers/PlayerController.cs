using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Helpers;
using server.Models.Core;
using server.Models.DTOs;

namespace server.Controllers;

[ApiController]
[Route("api/players")]
[Authorize] 
public class PlayersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PlayersController> _logger;

    public PlayersController(
        ApplicationDbContext context,
        ILogger<PlayersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// プレイヤー情報を取得
    /// </summary>
    /// <param name="playerId">プレイヤーID</param>
    /// <returns>プレイヤー情報</returns>
    [HttpGet("{playerId}")]
    public async Task<ActionResult<PlayerDto>> GetPlayer(string playerId)
    {
        try
        {
            // 認可チェック: 自分のデータのみアクセス可能
            if (!JwtHelper.IsAuthorized(User, playerId))
            {
                _logger.LogWarning("Unauthorized access attempt to player {PlayerId}", playerId);
                return Forbid();
            }

            var player = await _context.Players
                .AsNoTracking()
                .Where(p => p.PlayerId == playerId)
                .Select(p => new PlayerDto
                {
                    PlayerId = p.PlayerId,
                    Name = p.Name,
                    IconUrl = p.IconUrl
                })
                .FirstOrDefaultAsync();

            if (player == null)
            {
                return NotFound(new { message = $"Player with id {playerId} not found" });
            }

            return Ok(player);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting player {PlayerId}", playerId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// プレイヤーを新規登録
    /// </summary>
    /// <param name="dto">プレイヤー情報</param>
    /// <returns>作成されたプレイヤー情報</returns>
    [HttpPost]
    public async Task<ActionResult<PlayerDto>> CreatePlayer([FromBody] PlayerDto dto)
    {
        try
        {
            // 認可チェック: JWTのPlayerIdと一致するか
            var jwtPlayerId = JwtHelper.GetPlayerIdFromJwt(User);
            if (jwtPlayerId != dto.PlayerId)
            {
                _logger.LogWarning("Player ID mismatch. JWT: {JwtPlayerId}, Request: {RequestPlayerId}", 
                    jwtPlayerId, dto.PlayerId);
                return Forbid();
            }

            // 既に存在するか確認
            var existingPlayer = await _context.Players
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PlayerId == dto.PlayerId);

            if (existingPlayer != null)
            {
                return Conflict(new { message = "Player already exists" });
            }

            var player = new Player
            {
                PlayerId = dto.PlayerId,
                Name = dto.Name,
                IconUrl = dto.IconUrl
            };

            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Player created: {PlayerId}", player.PlayerId);

            return CreatedAtAction(
                nameof(GetPlayer),
                new { playerId = player.PlayerId },
                dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating player");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// プレイヤー情報を更新
    /// </summary>
    /// <param name="playerId">プレイヤーID</param>
    /// <param name="dto">更新するプレイヤー情報</param>
    /// <returns>更新後のプレイヤー情報</returns>
    [HttpPut("{playerId}")]
    public async Task<ActionResult<PlayerDto>> UpdatePlayer(
        string playerId,
        [FromBody] PlayerDto dto)
    {
        try
        {
            if (playerId != dto.PlayerId)
            {
                return BadRequest(new { message = "Player ID mismatch" });
            }

            // 認可チェック: 自分のデータのみ更新可能
            if (!JwtHelper.IsAuthorized(User, playerId))
            {
                _logger.LogWarning("Unauthorized update attempt to player {PlayerId}", playerId);
                return Forbid();
            }

            var player = await _context.Players
                .FirstOrDefaultAsync(p => p.PlayerId == playerId);

            if (player == null)
            {
                return NotFound(new { message = $"Player with id {playerId} not found" });
            }

            player.Name = dto.Name;
            player.IconUrl = dto.IconUrl;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Player updated: {PlayerId}", playerId);

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating player {PlayerId}", playerId);
            return StatusCode(500, "Internal server error");
        }
    }
}