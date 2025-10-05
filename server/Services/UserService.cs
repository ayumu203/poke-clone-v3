using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models.Core;
using System.Security.Claims;
using server.Interfaces;

namespace server.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Player> EnsureUserExistsAsync(TokenValidatedContext context)
        {
            var user = context.Principal;
            return await GetOrCreatePlayerAsync(user!);
        }

        public async Task<Player> GetOrCreatePlayerAsync(ClaimsPrincipal user)
        {
            // Azure ADからユーザー情報を取得
            var azureObjectId = user.FindFirst("oid")?.Value ?? 
                               user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            var email = user.FindFirst("email")?.Value ?? 
                       user.FindFirst("preferred_username")?.Value;
            var name = user.FindFirst("name")?.Value ?? 
                      user.FindFirst("given_name")?.Value ?? 
                      email?.Split('@')[0];

            if (string.IsNullOrEmpty(azureObjectId))
            {
                throw new UnauthorizedAccessException("Azure Object IDが取得できませんでした。");
            }

            // 既存ユーザーをチェック
            var existingPlayer = await GetPlayerByAzureIdAsync(azureObjectId);
            if (existingPlayer != null)
            {
                _logger.LogInformation("既存プレイヤーでログイン: {PlayerId}", existingPlayer.PlayerId);
                return existingPlayer;
            }

            // 新規ユーザーを作成
            var newPlayer = new Player
            {
                PlayerId = Guid.NewGuid().ToString(),
                Name = name ?? "Unknown User", 
                IconUrl = "https://example.com/default-avatar.png",
                AzureObjectId = azureObjectId,
                Email = email,
                CreatedAt = DateTime.UtcNow
            };

            _context.Players.Add(newPlayer);
            await _context.SaveChangesAsync();

            _logger.LogInformation("新規プレイヤーを作成: {PlayerId}, Azure ID: {AzureId}", 
                newPlayer.PlayerId, azureObjectId);

            return newPlayer;
        }

        public async Task<Player?> GetPlayerByAzureIdAsync(string azureObjectId)
        {
            return await _context.Players
                .FirstOrDefaultAsync(p => p.AzureObjectId == azureObjectId);
        }
    }
}