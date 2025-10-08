using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models.Core;

[ApiController]
[Route("api/[controller]")]
public class MoveController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public MoveController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet] 
    public async Task<ActionResult<IEnumerable<Move>>> GetMoves()
    {
        var moves = await _context.Moves.ToListAsync();
        return Ok(moves);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<Move>> GetMoveById(int id)
    {
        var move = await _context.Moves.FindAsync(id);

        if (move == null)
        {
            return NotFound();
        }

        return Ok(move);
    }
}