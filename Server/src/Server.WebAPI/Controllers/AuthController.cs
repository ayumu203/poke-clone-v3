using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Server.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

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
            UserId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            Name = User?.FindFirst(ClaimTypes.Name)?.Value,
            Message = isAuthenticated ? "Authenticated" : "Not Authenticated"
        });
    }

    [HttpPost("login/mock")]
    public IActionResult MockLogin([FromBody] MockLoginRequest request)
    {
        if (string.IsNullOrEmpty(request?.Username)) return BadRequest("Username is required");

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, request.Username),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        Response.Cookies.Append("access_token", tokenString, new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // HTTPS required for SameSite=None
            SameSite = SameSiteMode.None, // Required for cross-origin (localhost:3000 -> localhost:5278)
            Expires = DateTime.Now.AddDays(1)
        });

        return Ok(new { message = "Logged in", username = request.Username, token = tokenString });
    }

    public record MockLoginRequest(string Username, string? Password);

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("access_token", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });
        return Ok(new { message = "Logged out" });
    }
}
