using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models.Core;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PokemonMoveController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PokemonMoveController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("species/{speciesId}")]
        public async Task<ActionResult<IEnumerable<Move>>> GetMovesForPokemonSpecies(int speciesId)
        {
            var speciesExists = await _context.PokemonSpecies.AnyAsync(ps => ps.PokemonSpeciesId == speciesId);
            if (!speciesExists)
            {
                return NotFound($"PokemonSpecies with ID {speciesId} not found.");
            }

            var moves = await _context.PokemonMoves
                .Where(pm => pm.PokemonSpeciesId == speciesId)
                .Include(pm => pm.Move)
                .Select(pm => pm.Move)
                .ToListAsync();

            return Ok(moves);
        }
    }
}

