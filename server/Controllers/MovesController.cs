using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models.DTOs;

namespace server.Controllers;

[ApiController]
[Route("api/moves")]
[Authorize]
public class MovesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MovesController> _logger;

    public MovesController(
        ApplicationDbContext context,
        ILogger<MovesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 技一覧を取得
    /// </summary>
    /// <param name="skip">スキップする件数</param>
    /// <param name="take">取得する件数</param>
    /// <returns>技一覧</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MoveDto>>> GetMoves(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            var moves = await _context.Moves
                .AsNoTracking()
                .OrderBy(m => m.MoveId)
                .Skip(skip)
                .Take(take)
                .Select(m => new MoveDto
                {
                    MoveId = m.MoveId,
                    Name = m.Name,
                    Type = m.Type.ToString(),
                    Category = m.Category.ToString(),
                    DamageClass = m.DamageClass.ToString(),
                    Power = m.Power,
                    Accuracy = m.Accuracy,
                    Pp = m.PP,
                    Priority = m.Priority,
                    Rank = m.Rank,
                    StatTarget = m.StatTarget,
                    StatChance = m.StatChance,
                    Ailment = m.Ailment.ToString(),
                    AilmentChance = m.AilmentChance,
                    Healing = m.Healing,
                    Drain = m.Drain
                })
                .ToListAsync();

            return Ok(moves);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting moves list");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// 特定の技を取得
    /// </summary>
    /// <param name="id">技ID</param>
    /// <returns>技データ</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<MoveDto>> GetMove(int id)
    {
        try
        {
            var move = await _context.Moves
                .AsNoTracking()
                .Where(m => m.MoveId == id)
                .Select(m => new MoveDto
                {
                    MoveId = m.MoveId,
                    Name = m.Name,
                    Type = m.Type.ToString(),
                    Category = m.Category.ToString(),
                    DamageClass = m.DamageClass.ToString(),
                    Power = m.Power,
                    Accuracy = m.Accuracy,
                    Pp = m.PP,
                    Priority = m.Priority,
                    Rank = m.Rank,
                    StatTarget = m.StatTarget,
                    StatChance = m.StatChance,
                    Ailment = m.Ailment.ToString(),
                    AilmentChance = m.AilmentChance,
                    Healing = m.Healing,
                    Drain = m.Drain
                })
                .FirstOrDefaultAsync();

            if (move == null)
            {
                return NotFound(new { message = $"Move with id {id} not found" });
            }

            return Ok(move);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting move {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}