using Microsoft.AspNetCore.Authentication.JwtBearer;
using server.Models.Core;
using System.Security.Claims;

namespace server.Interfaces
{
    public interface IUserService
    {
        Task<Player> EnsureUserExistsAsync(TokenValidatedContext context);
        Task<Player> GetOrCreatePlayerAsync(ClaimsPrincipal user);
        Task<Player?> GetPlayerByAzureIdAsync(string azureObjectId);
    }
}