using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Server.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        var isAuthenticated = User?.Identity?.IsAuthenticated ?? false;
        return Ok(new 
        { 
            IsAuthenticated = isAuthenticated,
            UserId = User?.FindFirst("user_id")?.Value ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            Name = User?.FindFirst(ClaimTypes.Name)?.Value,
            Message = isAuthenticated ? "Authenticated" : "Not Authenticated"
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logged out" });
    }
}
