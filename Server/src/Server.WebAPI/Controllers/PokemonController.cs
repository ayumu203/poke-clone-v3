using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Domain.Repositories;

namespace Server.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PokemonController : ControllerBase
{
    private readonly IPokemonSpeciesRepository _pokemonSpeciesRepository;
    
    public PokemonController(IPokemonSpeciesRepository pokemonSpeciesRepository)
    {
        _pokemonSpeciesRepository = pokemonSpeciesRepository;
    }
    
    /// <summary>
    /// Get all Pokemon species
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var species = await _pokemonSpeciesRepository.GetAllAsync();
        return Ok(species);
    }
    
    /// <summary>
    /// Get a Pokemon species by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var species = await _pokemonSpeciesRepository.GetByIdAsync(id);
        if (species == null)
        {
            return NotFound();
        }
        return Ok(species);
    }
}
