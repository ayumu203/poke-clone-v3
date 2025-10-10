using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace server.Helpers;

/// <summary>
/// JWT関連のヘルパークラス
/// - 初期化に IConfiguration を受け取り、設定を一元化
/// - テスト用トークンの生成補助メソッドを提供
/// </summary>
public static class JwtHelper
{
    private static bool _disableAuthentication;
    private static string? _jwtKey;
    private static string? _issuer;
    private static string? _audience;

    /// <summary>
    /// JwtHelperを初期化（Program.csから呼ばれる）
    /// 設定から DisableAuthentication と Jwt セクション (Key/Issuer/Audience) を読み取ります。
    /// </summary>
    /// <summary>
    /// Initialize from configuration.
    /// If <paramref name="requireJwtSettings"/> is true, throws when Jwt:Key/Issuer/Audience are missing.
    /// </summary>
    public static void Initialize(IConfiguration configuration, bool requireJwtSettings = false)
    {
        _disableAuthentication = configuration.GetValue<bool>("DisableAuthentication");

        var jwtSection = configuration.GetSection("Jwt");
        _jwtKey = jwtSection.GetValue<string?>("Key");
        _issuer = jwtSection.GetValue<string?>("Issuer");
        _audience = jwtSection.GetValue<string?>("Audience");

        if (requireJwtSettings)
        {
            EnsureJwtSettings();
        }
    }

    /// <summary>
    /// JWTからPlayerIdを取得
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>PlayerId</returns>
    public static string? GetPlayerIdFromJwt(ClaimsPrincipal user)
    {
        // Azure Entra IDなどの一般的なクレームを順に参照
        var playerId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? user.FindFirst("sub")?.Value
                    ?? user.FindFirst("oid")?.Value;

        return playerId;
    }

    /// <summary>
    /// 安全にプレイヤーIDを取得します。成功すれば true を返し out に設定します。
    /// null/空の場合は false を返します。
    /// </summary>
    public static bool TryGetPlayerIdFromClaimsPrincipal(ClaimsPrincipal user, out string? playerId)
    {
        playerId = GetPlayerIdFromJwt(user);
        return !string.IsNullOrEmpty(playerId);
    }

    /// <summary>
    /// 認可チェック: 自分のデータかどうか
    /// 開発環境で認証が無効化されている場合は常にtrue
    /// </summary>
    public static bool IsAuthorized(ClaimsPrincipal user, string playerId)
    {
        if (_disableAuthentication)
        {
            return true;
        }

        if (TryGetPlayerIdFromClaimsPrincipal(user, out var currentPlayerId))
        {
            return currentPlayerId == playerId;
        }

        // クレームが取れない場合は認可失敗
        return false;
    }

    /// <summary>
    /// テスト用の JWT を発行する（ローカル開発用）。
    /// - 必要: appsettings の Jwt:Key が設定されていること
    /// - 発行するトークンは HS256 （対称鍵）を使用します
    /// </summary>
    public static string GenerateLocalJwt(string subject, TimeSpan? validFor = null)
    {
        EnsureJwtSettings();

        var keyBytes = System.Text.Encoding.UTF8.GetBytes(_jwtKey!);
        var securityKey = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var handler = new JwtSecurityTokenHandler();

        var now = DateTime.UtcNow;
        var expires = now.Add(validFor ?? TimeSpan.FromHours(1));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, subject),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: credentials
        );

        return handler.WriteToken(token);
    }

    /// <summary>
    /// 設定が揃っているか確認し、足りなければ InvalidOperationException を投げる
    /// </summary>
    private static void EnsureJwtSettings()
    {
        if (string.IsNullOrEmpty(_jwtKey) || string.IsNullOrEmpty(_issuer) || string.IsNullOrEmpty(_audience))
        {
            throw new InvalidOperationException("Jwt settings (Jwt:Key, Jwt:Issuer, Jwt:Audience) must be configured to use JWT-related helpers.");
        }
    }

    /// <summary>
    /// 文字列トークンを検証して ClaimsPrincipal を返します。検証失敗時は null を返します。
    /// 設定が不足している場合は例外を投げます。
    /// </summary>
    public static ClaimsPrincipal? ValidateTokenAndGetPrincipal(string token)
    {
        EnsureJwtSettings();

        var tokenHandler = new JwtSecurityTokenHandler();
        var keyBytes = System.Text.Encoding.UTF8.GetBytes(_jwtKey!);
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var _);
            return principal;
        }
        catch
        {
            return null;
        }
    }
}