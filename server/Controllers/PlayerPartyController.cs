using Microsoft.AspNetCore.Mvc;
using server.Data;
using server.Models;

[ApiController]
[Route("api/[controller]")]
public class PlayerPartyController : ControllerBase
{ 
    private readonly ApplicationDbContext _context;
    public PlayerPartyController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlayerParties()
    {
        var playerParties = _context.PlayerParties.ToList();
        return Ok(playerParties);
    }
}