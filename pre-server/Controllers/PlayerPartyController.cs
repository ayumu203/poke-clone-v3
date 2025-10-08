using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models.Core;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerPartyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PlayerPartyController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{playerId}")]
        public async Task<ActionResult<IEnumerable<Pokemon>>> GetPlayerPartyByPlayerId(string playerId)
        {
            var playerExists = await _context.Players.AnyAsync(p => p.PlayerId == playerId);
            if (!playerExists)
            {
                return NotFound($"Player with ID {playerId} not found.");
            }

            var partyPokemons = await _context.PlayerParties
                .Where(pp => pp.PlayerId == playerId)
                .OrderBy(pp => pp.SlotIndex)
                .Include(pp => pp.Pokemon)
                    .ThenInclude(p => p.PokemonSpecies)
                .Select(pp => pp.Pokemon)
                .ToListAsync();

            if (partyPokemons == null || !partyPokemons.Any())
            {
                return Ok(new List<Pokemon>());
            }

            return Ok(partyPokemons);
        }
    }
}
