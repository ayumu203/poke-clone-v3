using Microsoft.AspNetCore.Mvc;

namespace Server.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    /// <summary>
    /// Health check / Auth status endpoint
    /// </summary>
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        var isAuthenticated = User?.Identity?.IsAuthenticated ?? false;
        return Ok(new 
        { 
            IsAuthenticated = isAuthenticated,
            UserId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
            Message = "For development, use 'dotnet user-jwts create' to generate a JWT token"
        });
    }
}
