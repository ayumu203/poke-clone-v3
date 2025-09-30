using Microsoft.AspNetCore.Mvc;
using server.Data;
using server.Models;

[ApiController]
[Route("api/[controller]")]
public class PokemonsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public PokemonsController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPokemons()
    {
        var pokemons = _context.Pokemons.ToList();
        return Ok(pokemons);
    }
}

// 最初の2行ってC#のどんな文法的意味？