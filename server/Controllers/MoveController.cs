using Microsoft.AspNetCore.Mvc;
using server.Data;
using server.Models;

[ApiController]
[Route("api/[controller]")]
public class MoveController : ControllerBase
{ 
    private readonly ApplicationDbContext _context;
    public MoveController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMoves()
    {
        var moves = _context.Moves.ToList();
        return Ok(moves);
    }
}