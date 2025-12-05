using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Services;
using System.Security.Claims;

namespace Server.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BattleController : ControllerBase
{
    private readonly IBattleService _battleService;
    
    public BattleController(IBattleService battleService)
    {
        _battleService = battleService;
    }
    
    /// <summary>
    /// Create a new CPU battle
    /// </summary>
    [HttpPost("cpu")]
    public async Task<IActionResult> CreateCpuBattle()
    {
        var playerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(playerId))
        {
            return Unauthorized();
        }
        
        var battleState = await _battleService.CreateCpuBattleAsync(playerId);
        return Created($"/api/battle/{battleState.BattleId}", battleState);
    }
    
    /// <summary>
    /// Get battle state by ID
    /// </summary>
    [HttpGet("{battleId}")]
    public async Task<IActionResult> GetBattle(string battleId)
    {
        var battleState = await _battleService.GetBattleStateAsync(battleId);
        if (battleState == null)
        {
            return NotFound();
        }
        
        return Ok(battleState);
    }
}
