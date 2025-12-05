using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Domain.Entities;
using Server.Domain.Repositories;
using System.Security.Claims;

namespace Server.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlayerController : ControllerBase
{
    private readonly IPlayerRepository _playerRepository;
    
    public PlayerController(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }
    
    /// <summary>
    /// Get current player profile
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var playerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(playerId))
        {
            return Unauthorized();
        }
        
        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
        {
            return NotFound();
        }
        
        return Ok(player);
    }
    
    /// <summary>
    /// Create or update player profile
    /// </summary>
    [HttpPost("me")]
    public async Task<IActionResult> CreateOrUpdateMe([FromBody] PlayerDto dto)
    {
        var playerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(playerId))
        {
            return Unauthorized();
        }
        
        var existingPlayer = await _playerRepository.GetByIdAsync(playerId);
        if (existingPlayer != null)
        {
            // Update existing player
            existingPlayer.Name = dto.Name;
            existingPlayer.IconUrl = dto.IconUrl;
            await _playerRepository.UpdateAsync(existingPlayer);
            return Ok(existingPlayer);
        }
        else
        {
            // Create new player
            var newPlayer = new Player
            {
                PlayerId = playerId,
                Name = dto.Name,
                IconUrl = dto.IconUrl
            };
            await _playerRepository.AddAsync(newPlayer);
            return Created($"/api/player/me", newPlayer);
        }
    }
    
    public class PlayerDto
    {
        public string Name { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
    }
}
