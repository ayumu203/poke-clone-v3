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
    public class PokemonSpeciesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PokemonSpeciesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PokemonSpecies>>> GetPokemonSpeciesList()
        {
            var speciesList = await _context.PokemonSpecies.ToListAsync();
            return Ok(speciesList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PokemonSpecies>> GetPokemonSpeciesById(int id)
        {
            var pokemonSpecies = await _context.PokemonSpecies.FindAsync(id);

            if (pokemonSpecies == null)
            {
                return NotFound();
            }

            return Ok(pokemonSpecies);
        }
    }
}
