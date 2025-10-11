using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

if (args.Length < 2)
{
    Console.WriteLine("Usage: jwt-generator <key> <subject> [issuer] [audience] [hoursValid]");
    return 1;
}

var key = args[0];
var subject = args[1];
var issuer = args.Length >= 3 ? args[2] : "pokeclone.local";
var audience = args.Length >= 4 ? args[3] : "pokeclone.local";
var hoursValid = args.Length >= 5 && int.TryParse(args[4], out var h) ? h : 1;

var keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
var securityKey = new SymmetricSecurityKey(keyBytes);
var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

var handler = new JwtSecurityTokenHandler();

var now = DateTime.UtcNow;
var expires = now.AddHours(hoursValid);

var claims = new[]
{
    new Claim(JwtRegisteredClaimNames.Sub, subject),
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
};

var token = new JwtSecurityToken(
    issuer: issuer,
    audience: audience,
    claims: claims,
    notBefore: now,
    expires: expires,
    signingCredentials: credentials
);

var tokenString = handler.WriteToken(token);
Console.WriteLine(tokenString);

return 0;