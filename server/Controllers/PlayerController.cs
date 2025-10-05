using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models.Core;
using server.Interfaces;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PlayerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly ILogger<PlayerController> _logger;
        public PlayerController(
            ApplicationDbContext context, 
            IUserService userService,
            ILogger<PlayerController> logger)
        {
            _context = context;
            _userService = userService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
        {
            var players = await _context.Players.ToListAsync();
            return Ok(players);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Player>> GetPlayerById(string id)
        {
            var player = await _context.Players.FindAsync(id);

            if (player == null)
            {
                return NotFound();
            }

            return Ok(player);
        }
        [HttpGet("me")]
        public async Task<ActionResult<Player>> GetMyProfile()
        {
            try
            {
                var player = await _userService.GetOrCreatePlayerAsync(User);

                // 最終ログイン時刻を更新
                player.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(player);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdatePlayerRequest request)
        {
            try
            {
                var player = await _userService.GetOrCreatePlayerAsync(User);

                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    var nameExists = await _context.Players
                        .AnyAsync(p => p.Name == request.Name && p.PlayerId != player.PlayerId);

                    if (nameExists)
                    {
                        return Conflict("このプレイヤー名は既に使用されています。");
                    }

                    player.Name = request.Name;
                }

                if (!string.IsNullOrWhiteSpace(request.IconUrl))
                {
                    player.IconUrl = request.IconUrl;
                }

                await _context.SaveChangesAsync();
                return Ok(player);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
    public class UpdatePlayerRequest
    {
        public string? Name { get; set; }
        public string? IconUrl { get; set; }
    }
}
