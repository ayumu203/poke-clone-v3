using Microsoft.AspNetCore.Mvc;
using server.Data;
using server.Models;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public PlayerController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlayers()
    {
        var players = _context.Players.ToList();
        return Ok(players);
    }
}