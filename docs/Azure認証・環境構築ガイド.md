# Azure認証・環境構築ガイド

## 目次

1. [Azure認証アーキテクチャ概要](#azure認証アーキテクチャ概要)
2. [Azure Entra ID（旧Azure AD）設定](#azure-entra-id旧azure-ad設定)
3. [Azure Key Vault設定](#azure-key-vault設定)
4. [JWT認証の実装](#jwt認証の実装)
5. [Google OAuth認証の実装](#google-oauth認証の実装)
6. [Microsoft OAuth認証の実装](#microsoft-oauth認証の実装)
7. [開発環境と本番環境の統一](#開発環境と本番環境の統一)
8. [Azure App Serviceへのデプロイ](#azure-app-serviceへのデプロイ)

---

## Azure認証アーキテクチャ概要

### 本番環境構成図

```
┌────────────────────────────────────────────────────────────────────┐
│                           Azure Cloud                              │
│                                                                    │
│  ┌──────────────────┐         ┌──────────────────┐               │
│  │  Azure Entra     │         │   Azure Key      │               │
│  │  External ID     │◄────────┤   Vault          │               │
│  │  (認証基盤)      │  Secret │  (シークレット管理)│               │
│  │                  │  取得   │                  │               │
│  │  - Google OAuth  │         │  - JWT Secret    │               │
│  │  - Microsoft     │         │  - Client IDs    │               │
│  │  - 外部ID連携    │         │  - Client Secrets│               │
│  └────────┬─────────┘         │  - DB接続文字列  │               │
│           │                   └──────────┬───────┘               │
│           │ IDトークン                   │ Secret参照            │
│  ┌────────▼──────────────────────────────▼──────┐               │
│  │   Application Gateway (WAF)                  │               │
│  │   → API Management Service                   │               │
│  └────────┬──────────────────────────────────────┘               │
│           │                                                       │
│  ┌────────▼────────────────────────────┐                        │
│  │   Azure App Service                │                         │
│  │   (ASP.NET Core WebAPI)            │                         │
│  │   ┌──────────────────────────────┐ │                         │
│  │   │  認証ミドルウェア            │ │                         │
│  │   │  - JWT Bearer Auth           │ │                         │
│  │   │  - External Auth Handler     │ │                         │
│  │   └──────────────────────────────┘ │                         │
│  └─────────┬───────────┬────────────────┘                        │
│            │           │                                         │
│  ┌─────────▼─────┐  ┌──▼──────────────────┐                     │
│  │  Azure        │  │  Azure SignalR      │                     │
│  │  Database     │  │  Service            │                     │
│  │  for MySQL    │  │  (リアルタイム通信)  │                     │
│  └───────────────┘  └─────────────────────┘                     │
│                                                                  │
│  ┌──────────────────────┐                                       │
│  │  Azure Cache         │                                       │
│  │  for Redis           │                                       │
│  │  (バトル状態管理)     │                                       │
│  └──────────────────────┘                                       │
└────────────────────────────────────────────────────────────────────┘
                    ▲
                    │ HTTPS + WSS
                    │
          ┌─────────┴─────────┐
          │  フロントエンド    │
          │  (Next.js/React)   │
          │  - Vercel/Azure    │
          │    Static Web Apps │
          └────────────────────┘
```

### 開発環境構成図

```
┌─────────────────────────────────────────────────────────────┐
│                    開発マシン (localhost)                    │
│                                                               │
│  ┌──────────────────┐         ┌──────────────────┐          │
│  │  User Secrets    │         │   appsettings.   │          │
│  │  (secrets.json)  │         │   Development.   │          │
│  │                  │         │   json           │          │
│  │  - JWT Secret    │         │                  │          │
│  │  - Client IDs    │         │  - Mock Auth     │          │
│  │  - Client Secrets│         │  - Local DB      │          │
│  └────────┬─────────┘         └──────────┬───────┘          │
│           │                              │                  │
│           │ 読み込み                     │ 読み込み         │
│  ┌────────▼──────────────────────────────▼──────┐          │
│  │   ASP.NET Core WebAPI (localhost:5000)      │          │
│  │   ┌──────────────────────────────┐          │          │
│  │   │  開発用認証                  │          │          │
│  │   │  - Mock Login (開発専用)     │          │          │
│  │   │  - Google OAuth (任意)       │          │          │
│  │   │  - Microsoft OAuth (任意)    │          │          │
│  │   └──────────────────────────────┘          │          │
│  └─────────────┬────────────────────────────────┘          │
│                │                                             │
│  ┌─────────────▼─────────┐    ┌────────────────────┐       │
│  │  SQL Server (Docker)  │    │  Redis (Docker)    │       │
│  │  localhost:1433       │    │  localhost:6379    │       │
│  └───────────────────────┘    └────────────────────┘       │
│                                                               │
└─────────────────────────────────────────────────────────────┘
                    ▲
                    │ HTTP + WS (SignalR)
                    │
          ┌─────────┴─────────┐
          │  フロントエンド    │
          │  (Next.js/React)   │
          │  localhost:3000    │
          └────────────────────┘
```

---

## Azure Entra ID（旧Azure AD）設定

### 1. Entra IDアプリケーションの作成

#### Azure Portalでの手順

1. **Azure Portalにログイン**
   - https://portal.azure.com

2. **Microsoft Entra ID（旧Azure AD）に移動**
   ```
   Microsoft Entra ID → アプリの登録 → 新規登録
   ```

3. **アプリケーション登録**
   ```
   名前: poke-clone-api
   サポートされているアカウントの種類: 
     - シングルテナント（組織内のみ）
     または
     - マルチテナント + 個人用Microsoftアカウント
   
   リダイレクトURI: 
     - Web: https://your-app.azurewebsites.net/signin-oidc
     - SPA: https://your-frontend.vercel.app/auth/callback
   ```

4. **クライアントIDとテナントIDを記録**
   ```
   アプリケーション（クライアント）ID: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
   ディレクトリ（テナント）ID: yyyyyyyy-yyyy-yyyy-yyyy-yyyyyyyyyyyy
   ```

5. **クライアントシークレットを作成**
   ```
   証明書とシークレット → 新しいクライアントシークレット
   説明: poke-clone-secret
   有効期限: 24か月（推奨）
   
   → シークレット値をコピー（一度しか表示されない！）
   ```

### 2. API権限の設定

```
APIのアクセス許可 → アクセス許可の追加

Microsoft Graph:
  - User.Read
  - email
  - openid
  - profile

管理者の同意が必要な場合:
  → "○○に管理者の同意を与えます"をクリック
```

### 3. 公開スコープの設定

```
APIの公開 → スコープの追加

スコープ名: access_as_user
管理者の同意の表示名: Access poke-clone API
管理者の同意の説明: Allows the app to access poke-clone API as the user
状態: 有効
```

---

## Azure Key Vault設定

### 1. Key Vaultの作成

#### Azure CLIでの作成

```bash
# Azure CLIログイン
az login

# リソースグループ作成
az group create \
  --name poke-clone-rg \
  --location japaneast

# Key Vault作成
az keyvault create \
  --name poke-clone-keyvault \
  --resource-group poke-clone-rg \
  --location japaneast \
  --enable-rbac-authorization false
```

#### Azure Portalでの作成

```
1. Key Vault → 作成
2. リソースグループ: poke-clone-rg
3. Key Vault名: poke-clone-keyvault
4. リージョン: Japan East
5. 価格レベル: Standard
6. アクセスポリシー: 
   - 自分のアカウントに全権限を付与
   - App ServiceマネージドIDにシークレット読み取り権限を付与
```

### 2. シークレットの登録

#### Azure CLIでのシークレット登録

```bash
# JWTシークレットキー
az keyvault secret set \
  --vault-name poke-clone-keyvault \
  --name JwtSettings--SecretKey \
  --value "your-super-secure-jwt-secret-key-minimum-32-characters"

# Google OAuth
az keyvault secret set \
  --vault-name poke-clone-keyvault \
  --name Authentication--Google--ClientId \
  --value "xxxx.apps.googleusercontent.com"

az keyvault secret set \
  --vault-name poke-clone-keyvault \
  --name Authentication--Google--ClientSecret \
  --value "GOCSPX-xxxxxxxxxxxxx"

# Microsoft OAuth
az keyvault secret set \
  --vault-name poke-clone-keyvault \
  --name Authentication--Microsoft--ClientId \
  --value "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"

az keyvault secret set \
  --vault-name poke-clone-keyvault \
  --name Authentication--Microsoft--ClientSecret \
  --value "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"

# データベース接続文字列 (SQL Server)
az keyvault secret set \
  --vault-name poke-clone-keyvault \
  --name ConnectionStrings--DefaultConnection \
  --value "Server=tcp:poke-clone-db-server.database.windows.net,1433;Initial Catalog=pokedb;Persist Security Info=False;User ID=sqladmin;Password=YourPassword123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

# Redis接続文字列
az keyvault secret set \
  --vault-name poke-clone-keyvault \
  --name ConnectionStrings--Redis \
  --value "poke-clone-redis.redis.cache.windows.net:6380,password=xxxxxxxxx,ssl=True,abortConnect=False"

# Azure SignalR接続文字列
az keyvault secret set \
  --vault-name poke-clone-keyvault \
  --name Azure--SignalR--ConnectionString \
  --value "Endpoint=https://poke-clone-signalr.service.signalr.net;AccessKey=xxxxxxxxx;Version=1.0;"
```

### 3. App ServiceマネージドIDの設定

```bash
# App Serviceのマネージドアイデンティティを有効化
az webapp identity assign \
  --name poke-clone-api \
  --resource-group poke-clone-rg

# 出力されたprincipalIdを記録
# "principalId": "zzzzzzzz-zzzz-zzzz-zzzz-zzzzzzzzzzzz"

# Key Vaultアクセスポリシーを設定
az keyvault set-policy \
  --name poke-clone-keyvault \
  --object-id zzzzzzzz-zzzz-zzzz-zzzz-zzzzzzzzzzzz \
  --secret-permissions get list
```

---

## JWT認証の実装

### 1. 開発環境設定

#### User Secretsの設定（開発環境）

```bash
# プロジェクトディレクトリに移動
cd Server/src/Server.WebAPI

# User Secrets初期化
dotnet user-secrets init

# JWTシークレット設定
dotnet user-secrets set "JwtSettings:SecretKey" "your-development-jwt-secret-key-32-chars-min"
dotnet user-secrets set "JwtSettings:Issuer" "http://localhost:5000"
dotnet user-secrets set "JwtSettings:Audience" "http://localhost:3000"
dotnet user-secrets set "JwtSettings:ExpiryMinutes" "60"

# Google OAuth（開発用）
dotnet user-secrets set "Authentication:Google:ClientId" "your-google-client-id.apps.googleusercontent.com"
dotnet user-secrets set "Authentication:Google:ClientSecret" "GOCSPX-your-google-client-secret"

# Microsoft OAuth（開発用）
dotnet user-secrets set "Authentication:Microsoft:ClientId" "your-microsoft-client-id"
dotnet user-secrets set "Authentication:Microsoft:ClientSecret" "your-microsoft-client-secret"
```

#### appsettings.Development.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.AspNetCore.SignalR": "Debug"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=PokeCloneDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;",
    "Redis": "localhost:6379,abortConnect=false"
  },
  "Azure": {
    "SignalR": {
      "ConnectionString": ""
    }
  },
  "JwtSettings": {
    "Issuer": "http://localhost:5000",
    "Audience": "http://localhost:3000",
    "ExpiryMinutes": 60
  },
  "Authentication": {
    "Google": {
      "CallbackPath": "/signin-google"
    },
    "Microsoft": {
      "CallbackPath": "/signin-microsoft"
    }
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000", "http://localhost:3001"]
  }
}
```

### 2. 本番環境設定

#### appsettings.Production.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*.azurewebsites.net",
  "KeyVault": {
    "VaultUri": "https://poke-clone-keyvault.vault.azure.net/"
  },
  "JwtSettings": {
    "Issuer": "https://poke-clone-api.azurewebsites.net",
    "Audience": "https://poke-clone-frontend.vercel.app",
    "ExpiryMinutes": 60
  },
  "Authentication": {
    "Google": {
      "CallbackPath": "/signin-google"
    },
    "Microsoft": {
      "CallbackPath": "/signin-microsoft"
    }
  },
  "Cors": {
    "AllowedOrigins": ["https://poke-clone-frontend.vercel.app"]
  }
}
```

### 3. Program.csの実装

#### 完全なProgram.cs（開発・本番統一）

```csharp
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Infrastructure.Data;
using Server.Application.Services;
using Server.Domain.Repositories;
using Server.Domain.Services;
using Server.Infrastructure.Repositories;
using Server.Infrastructure.Services;
using Server.WebAPI.Hubs;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// Azure Key Vault設定（本番環境のみ）
// ========================================
if (builder.Environment.IsProduction())
{
    var keyVaultUri = builder.Configuration["KeyVault:VaultUri"];
    if (!string.IsNullOrEmpty(keyVaultUri))
    {
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUri),
            new DefaultAzureCredential());
    }
}

// ========================================
// データベース設定 (SQL Server)
// ========================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

// ========================================
// Redis設定
// ========================================
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("Redis");
    return ConnectionMultiplexer.Connect(configuration);
});

// ========================================
// JWT認証設定
// ========================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = builder.Environment.IsProduction();
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    
    // SignalR用のトークン取得
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            
            return Task.CompletedTask;
        }
    };
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.CallbackPath = builder.Configuration["Authentication:Google:CallbackPath"];
})
.AddMicrosoftAccount(options =>
{
    options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
    options.CallbackPath = builder.Configuration["Authentication:Microsoft:CallbackPath"];
});

builder.Services.AddAuthorization();

// ========================================
// CORS設定
// ========================================
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ========================================
// DI登録
// ========================================
// ========================================
// SignalR設定
// ========================================
if (builder.Environment.IsProduction())
{
    // Azure SignalR Serviceを使用
    var signalRConnectionString = builder.Configuration["Azure:SignalR:ConnectionString"];
    builder.Services.AddSignalR().AddAzureSignalR(signalRConnectionString);
}
else
{
    // 開発環境はセルフホスト
    builder.Services.AddSignalR();
}

var app = builder.Build();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IPokemonSpeciesRepository, PokemonSpeciesRepository>();
builder.Services.AddScoped<IMoveRepository, MoveRepository>();
builder.Services.AddScoped<IBattleRepository, RedisBattleRepository>();

// Services
builder.Services.AddScoped<IDamageCalculator, DamageCalculator>();
builder.Services.AddScoped<ITypeEffectivenessManager, TypeEffectivenessManager>();
builder.Services.AddScoped<IStatCalculator, StatCalculator>();
builder.Services.AddScoped<IBattleService, BattleService>();

var app = builder.Build();

// ========================================
// ミドルウェア設定
// ========================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<BattleHub>("/hubs/battle");

// データベース初期化
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await SeedData.Initialize(context);
}

app.Run();
```

### 4. AuthController拡張（外部認証対応）

#### Controllers/AuthController.cs

```csharp
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authorization;
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
    private readonly ILogger<AuthController> _logger;

    public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    // ========================================
    // Google OAuth認証
    // ========================================
    [HttpGet("google/login")]
    public IActionResult GoogleLogin(string returnUrl = "/")
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(GoogleCallback), new { returnUrl })
        };
        
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleCallback(string returnUrl = "/")
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        
        if (!authenticateResult.Succeeded)
        {
            _logger.LogWarning("Google authentication failed");
            return Redirect($"{GetFrontendUrl()}/login?error=google_auth_failed");
        }

        var claims = authenticateResult.Principal.Claims.ToList();
        var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var googleId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        // ユーザー登録/取得処理（実装必要）
        // var user = await GetOrCreateUser(email, name, "google", googleId);

        // JWT生成
        var token = GenerateJwtToken(email, name, "google");

        // フロントエンドにリダイレクト
        return Redirect($"{GetFrontendUrl()}/auth/callback?token={token}");
    }

    // ========================================
    // Microsoft OAuth認証
    // ========================================
    [HttpGet("microsoft/login")]
    public IActionResult MicrosoftLogin(string returnUrl = "/")
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(MicrosoftCallback), new { returnUrl })
        };
        
        return Challenge(properties, MicrosoftAccountDefaults.AuthenticationScheme);
    }

    [HttpGet("microsoft/callback")]
    public async Task<IActionResult> MicrosoftCallback(string returnUrl = "/")
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(MicrosoftAccountDefaults.AuthenticationScheme);
        
        if (!authenticateResult.Succeeded)
        {
            _logger.LogWarning("Microsoft authentication failed");
            return Redirect($"{GetFrontendUrl()}/login?error=microsoft_auth_failed");
        }

        var claims = authenticateResult.Principal.Claims.ToList();
        var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var microsoftId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        // JWT生成
        var token = GenerateJwtToken(email, name, "microsoft");

        // フロントエンドにリダイレクト
        return Redirect($"{GetFrontendUrl()}/auth/callback?token={token}");
    }

    // ========================================
    // JWT直接ログイン（開発環境専用）
    // ========================================
    [HttpPost("login/mock")]
    public IActionResult MockLogin([FromBody] LoginRequest request)
    {
        if (!IsDevelopmentEnvironment())
        {
            return NotFound();
        }

        // 開発環境のみで使用可能
        var token = GenerateJwtToken(request.Username, request.Username, "mock");

        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // 開発環境はHTTP
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(60)
        });

        return Ok(new
        {
            message = "Login successful",
            username = request.Username,
            token = token
        });
    }

    // ========================================
    // トークン検証エンドポイント
    // ========================================
    [Authorize]
    [HttpGet("verify")]
    public IActionResult VerifyToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var name = User.FindFirst(ClaimTypes.Name)?.Value;

        return Ok(new
        {
            userId,
            email,
            name,
            isAuthenticated = true
        });
    }

    // ========================================
    // トークンリフレッシュ
    // ========================================
    [Authorize]
    [HttpPost("refresh")]
    public IActionResult RefreshToken()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        var provider = User.FindFirst("provider")?.Value ?? "jwt";

        var newToken = GenerateJwtToken(email, name, provider);

        return Ok(new { token = newToken });
    }

    // ========================================
    // ログアウト
    // ========================================
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return Ok(new { message = "Logged out successfully" });
    }

    // ========================================
    // ヘルパーメソッド
    // ========================================
    private string GenerateJwtToken(string email, string name, string provider)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, email ?? ""),
                new Claim(ClaimTypes.Name, name ?? ""),
                new Claim("provider", provider),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpiryMinutes"])),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GetFrontendUrl()
    {
        return IsDevelopmentEnvironment()
            ? "http://localhost:3000"
            : _configuration["JwtSettings:Audience"];
    }

    private bool IsDevelopmentEnvironment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
```

---

## Google OAuth認証の実装

### 1. Google Cloud Console設定

1. **Google Cloud Consoleにアクセス**
   - https://console.cloud.google.com/

2. **新しいプロジェクト作成**
   ```
   プロジェクト名: poke-clone
   ```

3. **OAuth同意画面の設定**
   ```
   APIとサービス → OAuth同意画面
   
   ユーザータイプ: 外部
   アプリ名: PokeClone
   ユーザーサポートメール: your-email@example.com
   承認済みドメイン: 
     - localhost (開発用)
     - azurewebsites.net (本番用)
     - vercel.app (フロントエンド)
   
   スコープ:
     - email
     - profile
     - openid
   ```

4. **OAuth 2.0クライアントIDの作成**
   ```
   APIとサービス → 認証情報 → 認証情報を作成 → OAuth クライアントID
   
   アプリケーションの種類: ウェブアプリケーション
   名前: poke-clone-web
   
   承認済みのJavaScript生成元:
     - http://localhost:3000
     - https://poke-clone-frontend.vercel.app
   
   承認済みのリダイレクトURI:
     - http://localhost:5000/signin-google
     - https://poke-clone-api.azurewebsites.net/signin-google
   
   → クライアントIDとクライアントシークレットが生成される
   ```

### 2. フロントエンド実装（Next.js）

#### pages/login.tsx

```typescript
import React from 'react';
import { useRouter } from 'next/router';

export default function LoginPage() {
  const router = useRouter();
  const apiUrl = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';

  const handleGoogleLogin = () => {
    // バックエンドのGoogle OAuth開始エンドポイントにリダイレクト
    window.location.href = `${apiUrl}/api/Auth/google/login`;
  };

  const handleMicrosoftLogin = () => {
    window.location.href = `${apiUrl}/api/Auth/microsoft/login`;
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100">
      <div className="bg-white p-8 rounded-lg shadow-md w-96">
        <h1 className="text-2xl font-bold mb-6 text-center">ログイン</h1>
        
        <div className="space-y-4">
          <button
            onClick={handleGoogleLogin}
            className="w-full flex items-center justify-center gap-2 bg-white border border-gray-300 text-gray-700 py-2 px-4 rounded hover:bg-gray-50"
          >
            <img src="/google-icon.svg" alt="Google" className="w-5 h-5" />
            Googleでログイン
          </button>

          <button
            onClick={handleMicrosoftLogin}
            className="w-full flex items-center justify-center gap-2 bg-white border border-gray-300 text-gray-700 py-2 px-4 rounded hover:bg-gray-50"
          >
            <img src="/microsoft-icon.svg" alt="Microsoft" className="w-5 h-5" />
            Microsoftでログイン
          </button>

          {process.env.NODE_ENV === 'development' && (
            <button
              onClick={() => handleMockLogin()}
              className="w-full bg-gray-600 text-white py-2 px-4 rounded hover:bg-gray-700"
            >
              開発用ログイン
            </button>
          )}
        </div>
      </div>
    </div>
  );
}

async function handleMockLogin() {
  const response = await fetch('http://localhost:5000/api/Auth/login/mock', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username: 'devuser', password: 'devpass' })
  });
  
  const data = await response.json();
  localStorage.setItem('jwt_token', data.token);
  window.location.href = '/';
}
```

#### pages/auth/callback.tsx

```typescript
import { useEffect } from 'react';
import { useRouter } from 'next/router';

export default function AuthCallbackPage() {
  const router = useRouter();
  const { token, error } = router.query;

  useEffect(() => {
    if (token) {
      // JWTトークンを保存
      localStorage.setItem('jwt_token', token as string);
      
      // ホームページにリダイレクト
      router.push('/');
    } else if (error) {
      // エラーメッセージを表示してログインページに戻る
      alert(`認証エラー: ${error}`);
      router.push('/login');
    }
  }, [token, error, router]);

  return (
    <div className="min-h-screen flex items-center justify-center">
      <div className="text-center">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mx-auto mb-4"></div>
        <p>認証処理中...</p>
      </div>
    </div>
  );
}
```

---

## Azure SignalR Serviceの設定

### 1. SignalR Serviceの作成

```bash
# Azure SignalR Service作成
az signalr create \
  --name poke-clone-signalr \
  --resource-group poke-clone-rg \
  --location japaneast \
  --sku Standard_S1 \
  --service-mode Default

# 接続文字列の取得
az signalr key list \
  --name poke-clone-signalr \
  --resource-group poke-clone-rg \
  --query primaryConnectionString \
  --output tsv
```

### 2. ASP.NET Coreでの使用

#### NuGetパッケージのインストール

```bash
cd Server/src/Server.WebAPI
dotnet add package Microsoft.Azure.SignalR
```

#### Program.csでの設定

前述のProgram.csを参照。本番環境では`AddAzureSignalR()`を使用。

#### appsettings.Production.json

```json
{
  "Azure": {
    "SignalR": {
      "ConnectionString": "@Microsoft.KeyVault(SecretUri=https://poke-clone-keyvault.vault.azure.net/secrets/Azure--SignalR--ConnectionString/)"
    }
  }
}
```

### 3. フロントエンド接続

フロントエンドからは同じSignalRクライアントを使用します。エンドポイントURLが変わるだけです：

```typescript
// 本番環境
const connection = new HubConnectionBuilder()
  .withUrl("https://poke-clone-api.azurewebsites.net/hubs/battle", {
    accessTokenFactory: () => localStorage.getItem('jwt_token') || ''
  })
  .withAutomaticReconnect()
  .build();
```

### 4. メリット

- **スケーラビリティ**: 自動スケーリングで大量の同時接続に対応
- **高可用性**: Azureが管理するフルマネージドサービス
- **複数インスタンス対応**: App Serviceの複数インスタンス間でSignalRメッセージを自動同期
- **WebSocketサポート**: 完全なWebSocketサポートと自動フォールバック

---

## Microsoft OAuth認証の実装

### 1. Microsoft Entra ID設定（再掲）

前述の「Azure Entra ID設定」セクションを参照

### 2. 追加設定

#### リダイレクトURIの追加

```
Azure Portal → Microsoft Entra ID → アプリの登録 → poke-clone-api

認証 → プラットフォームの追加 → Web

リダイレクトURI:
  - http://localhost:5000/signin-microsoft (開発)
  - https://poke-clone-api.azurewebsites.net/signin-microsoft (本番)
```

#### トークン構成

```
トークン構成 → オプションの要求を追加

トークンの種類: ID
要求:
  - email
  - family_name
  - given_name
  - upn
```

---

## 開発環境と本番環境の統一

### 環境変数管理戦略

#### 開発環境

```bash
# .env.local (Gitにコミットしない)
NEXT_PUBLIC_API_URL=http://localhost:5000
NEXT_PUBLIC_SIGNALR_URL=http://localhost:5000/hubs/battle
NEXT_PUBLIC_ENVIRONMENT=development
```

```bash
# User Secrets (dotnet)
dotnet user-secrets set "JwtSettings:SecretKey" "dev-secret-key-32-chars"
dotnet user-secrets set "Authentication:Google:ClientId" "dev-google-id"
dotnet user-secrets set "Authentication:Google:ClientSecret" "dev-google-secret"
```

#### 本番環境

```bash
# Azure App Service環境変数
az webapp config appsettings set \
  --name poke-clone-api \
  --resource-group poke-clone-rg \
  --settings \
    JwtSettings__SecretKey="@Microsoft.KeyVault(SecretUri=https://poke-clone-keyvault.vault.azure.net/secrets/JwtSettings--SecretKey/)" \
    Authentication__Google__ClientId="@Microsoft.KeyVault(SecretUri=https://poke-clone-keyvault.vault.azure.net/secrets/Authentication--Google--ClientId/)" \
    Authentication__Google__ClientSecret="@Microsoft.KeyVault(SecretUri=https://poke-clone-keyvault.vault.azure.net/secrets/Authentication--Google--ClientSecret/)"
```

### Docker Compose統一環境

#### docker-compose.yml（完全版）

```yaml
version: '3.8'

services:
  # SQL Server
  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: pokeclone_db
    environment:
      - ACCEPT_EULA=Y
      - MSSQL_SA_PASSWORD=YourStrong@Passw0rd
      - MSSQL_PID=Developer
    ports:
      - "1433:1433"
    volumes:
      - sqlserver_data:/var/opt/mssql
    networks:
      - pokeclone_network
    healthcheck:
      test: /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -C -Q "SELECT 1" || exit 1
      interval: 10s
      timeout: 5s
      retries: 5
      start_period: 30s

  # Redis
  redis:
    image: redis:7-alpine
    container_name: pokeclone_redis
    ports:
      - "6379:6379"
    networks:
      - pokeclone_network
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 5s
      retries: 5

  # ASP.NET Core API
  app:
    build:
      context: ./Server
      dockerfile: Dockerfile
    container_name: pokeclone_app
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:5000
      - ConnectionStrings__DefaultConnection=Server=db,1433;Database=PokeCloneDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;
      - ConnectionStrings__Redis=redis:6379
      - JwtSettings__SecretKey=your-development-jwt-secret-key-32-chars-minimum
      - JwtSettings__Issuer=http://localhost:5000
      - JwtSettings__Audience=http://localhost:3000
      - JwtSettings__ExpiryMinutes=60
      - IsAuthenticationEnabled=true
    ports:
      - "5000:5000"
    depends_on:
      db:
        condition: service_healthy
      redis:
        condition: service_healthy
    networks:
      - pokeclone_network
    volumes:
      - ./Server/Data:/app/Data
      - ./Docs/seeds:/seeds

volumes:
  sqlserver_data:

networks:
  pokeclone_network:
    driver: bridge
```

---

## Azure App Serviceへのデプロイ

### 1. Azure CLIでのリソース作成

```bash
# リソースグループ作成
az group create \
  --name poke-clone-rg \
  --location japaneast

# App Service Plan作成
az appservice plan create \
  --name poke-clone-plan \
  --resource-group poke-clone-rg \
  --sku B1 \
  --is-linux

# Web App作成
az webapp create \
  --name poke-clone-api \
  --resource-group poke-clone-rg \
  --plan poke-clone-plan \
  --runtime "DOTNETCORE:8.0"

# Azure SQL Database作成
az sql server create \
  --name poke-clone-db-server \
  --resource-group poke-clone-rg \
  --location japaneast \
  --admin-user sqladmin \
  --admin-password YourPassword123!

az sql db create \
  --name pokedb \
  --resource-group poke-clone-rg \
  --server poke-clone-db-server \
  --service-objective S0

# Azure Cache for Redis作成
az redis create \
  --name poke-clone-redis \
  --resource-group poke-clone-rg \
  --location japaneast \
  --sku Basic \
  --vm-size c0

# Azure SignalR Service作成
az signalr create \
  --name poke-clone-signalr \
  --resource-group poke-clone-rg \
  --location japaneast \
  --sku Standard_S1 \
  --service-mode Default

# Key Vault作成
az keyvault create \
  --name poke-clone-keyvault \
  --resource-group poke-clone-rg \
  --location japaneast
```

### 2. GitHub Actionsでのデプロイ

#### .github/workflows/azure-deploy.yml

```yaml
name: Deploy to Azure

on:
  push:
    branches:
      - main
  workflow_dispatch:

env:
  AZURE_WEBAPP_NAME: poke-clone-api
  DOTNET_VERSION: '8.0.x'

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Restore dependencies
      run: dotnet restore Server/PokeClone.sln
    
    - name: Build
      run: dotnet build Server/PokeClone.sln --configuration Release --no-restore
    
    - name: Publish
      run: dotnet publish Server/src/Server.WebAPI/Server.WebAPI.csproj --configuration Release --no-build --output ./publish
    
    - name: Deploy to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: ${{ env.AZURE_WEBAPP_NAME }}
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ./publish
```

### 3. App Service環境変数設定

```bash
# 環境変数設定（Key Vault参照）
az webapp config appsettings set \
  --name poke-clone-api \
  --resource-group poke-clone-rg \
  --settings \
    ASPNETCORE_ENVIRONMENT=Production \
    KeyVault__VaultUri=https://poke-clone-keyvault.vault.azure.net/ \
    ConnectionStrings__DefaultConnection="@Microsoft.KeyVault(SecretUri=https://poke-clone-keyvault.vault.azure.net/secrets/ConnectionStrings--DefaultConnection/)" \
    ConnectionStrings__Redis="@Microsoft.KeyVault(SecretUri=https://poke-clone-keyvault.vault.azure.net/secrets/ConnectionStrings--Redis/)" \
    JwtSettings__SecretKey="@Microsoft.KeyVault(SecretUri=https://poke-clone-keyvault.vault.azure.net/secrets/JwtSettings--SecretKey/)"
```

---

## フロントエンド環境変数管理

### Vercelデプロイ設定

#### vercel.json

```json
{
  "buildCommand": "npm run build",
  "outputDirectory": ".next",
  "framework": "nextjs",
  "env": {
    "NEXT_PUBLIC_API_URL": "https://poke-clone-api.azurewebsites.net",
    "NEXT_PUBLIC_SIGNALR_URL": "https://poke-clone-api.azurewebsites.net/hubs/battle",
    "NEXT_PUBLIC_ENVIRONMENT": "production"
  }
}
```

### 環境別設定ファイル

#### .env.development

```bash
NEXT_PUBLIC_API_URL=http://localhost:5000
NEXT_PUBLIC_SIGNALR_URL=http://localhost:5000/hubs/battle
NEXT_PUBLIC_ENVIRONMENT=development
NEXT_PUBLIC_ENABLE_MOCK_AUTH=true
```

#### .env.production

```bash
NEXT_PUBLIC_API_URL=https://poke-clone-api.azurewebsites.net
NEXT_PUBLIC_SIGNALR_URL=https://poke-clone-api.azurewebsites.net/hubs/battle
NEXT_PUBLIC_ENVIRONMENT=production
NEXT_PUBLIC_ENABLE_MOCK_AUTH=false
```

---

## セキュリティチェックリスト

### 開発環境

- [ ] User Secretsを使用してシークレットを管理
- [ ] appsettings.Development.jsonに機密情報を含めない
- [ ] Gitに`.env`ファイルをコミットしない
- [ ] HTTPS必須を無効化（開発環境のみ）
- [ ] CORS設定をlocalhostに限定

### 本番環境

- [ ] Azure Key Vaultですべてのシークレットを管理
- [ ] マネージドIDを使用してKey Vaultにアクセス
- [ ] HTTPS必須を有効化
- [ ] JWT SecretKeyは32文字以上のランダム文字列
- [ ] CORS設定を本番ドメインのみに限定
- [ ] Redisにパスワードを設定
- [ ] SQL DatabaseでAzure AD認証を有効化
- [ ] App ServiceでHTTPS Only設定を有効化
- [ ] 定期的にシークレットをローテーション

---

## トラブルシューティング

### Key Vault接続エラー

**症状**: `DefaultAzureCredential failed to retrieve a token`

**解決策**:
```bash
# マネージドIDが有効か確認
az webapp identity show --name poke-clone-api --resource-group poke-clone-rg

# Key Vaultアクセスポリシーを確認
az keyvault show --name poke-clone-keyvault --resource-group poke-clone-rg
```

### Google OAuth認証失敗

**症状**: `redirect_uri_mismatch`

**解決策**:
- Google Cloud Consoleで登録したリダイレクトURIとコードが一致しているか確認
- プロトコル（http/https）、ポート番号も含めて完全一致させる

### SignalR接続失敗（本番環境）

**症状**: `WebSocket connection failed`

**解決策**:
```bash
# App ServiceでWebSocketsを有効化
az webapp config set \
  --name poke-clone-api \
  --resource-group poke-clone-rg \
  --web-sockets-enabled true
```

---

## まとめ

このドキュメントでは、Azure Entra ID、Key Vault、JWT認証、外部OAuth認証（Google/Microsoft）の実装方法と、開発環境と本番環境を統一するための設定を説明しました。

**重要なポイント**:

1. **Key Vaultで統一管理**: 本番環境のすべてのシークレットをKey Vaultで管理
2. **User Secretsで開発**: 開発環境はUser Secretsを使い、Gitにコミットしない
3. **マネージドIDの活用**: App ServiceからKey Vaultへのアクセスは認証情報不要
4. **環境変数の統一**: `appsettings.json`と環境変数で同じ構造を維持
5. **OAuth統合**: Google/Microsoft認証をJWT発行に統一

これで開発環境も本番環境も同じアーキテクチャで動作します！🚀
