using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Domain.Repositories;

namespace Server.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovesController : ControllerBase
{
    private readonly IMoveRepository _moveRepository;
    
    public MovesController(IMoveRepository moveRepository)
    {
        _moveRepository = moveRepository;
    }
    
    /// <summary>
    /// Get all moves
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var moves = await _moveRepository.GetAllAsync();
        return Ok(moves);
    }
    
    /// <summary>
    /// Get a move by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var move = await _moveRepository.GetByIdAsync(id);
        if (move == null)
        {
            return NotFound();
        }
        return Ok(move);
    }
}
