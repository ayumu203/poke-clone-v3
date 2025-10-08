using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PokemonsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PokemonsController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pokemon>>> GetPokemons()
        {
            var pokemons = await _context.Pokemons.ToListAsync();
            return Ok(pokemons);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pokemon>> GetPokemonById(int id)
        {
            var pokemon = await _context.Pokemons
                .Include(p => p.PokemonSpecies) 
                .Include(p => p.Move1)         
                .Include(p => p.Move2)          
                .Include(p => p.Move3)          
                .Include(p => p.Move4)          
                .FirstOrDefaultAsync(p => p.PokemonId == id);

            if (pokemon == null)
            {
                return NotFound();
            }

            return Ok(pokemon);
        }
    }
}
