using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace server.Services;

public class NoAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    // Updated to avoid obsolete ISystemClock usage; AuthenticationHandler offers constructor overloads
    public NoAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // 開発用にデフォルトのClaimsPrincipalを返す
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "dev-user") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
