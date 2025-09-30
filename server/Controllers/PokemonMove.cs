using Microsoft.AspNetCore.Mvc;
using server.Data;
using server.Models;

[ApiController]
[Route("api/[controller]")]
public class PokemonMoveController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public PokemonMoveController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPokemonMoves(int id)
    {
        var pokemonMoves = _context.PokemonMoves.ToList();
        return Ok(pokemonMoves);
    }
}