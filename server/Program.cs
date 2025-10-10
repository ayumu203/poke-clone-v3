using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using server.Data;
using server.Services;
using server.Helpers;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // JSON循環参照を無視
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// JWT認証設定
// 新しい設定名: IsAuthenticationEnabled (bool)
// 互換のため既存の DisableAuthentication (bool) が残っている場合は両方をチェックします。
// 優先順: IsAuthenticationEnabled (if present) -> !DisableAuthentication
var hasIsAuth = builder.Configuration.GetChildren().Any(c => string.Equals(c.Key, "IsAuthenticationEnabled", StringComparison.OrdinalIgnoreCase));
var isAuthenticationEnabled = hasIsAuth
    ? builder.Configuration.GetValue<bool>("IsAuthenticationEnabled")
    : !builder.Configuration.GetValue<bool>("DisableAuthentication", false);

// JwtHelperを初期化（設定からDisableAuthentication / Jwtセクションを読み取る）
JwtHelper.Initialize(builder.Configuration);

if (isAuthenticationEnabled)
{
    // ローカルでの簡易JWTを使うか、Azure Entra ID を使うかを設定で切り替えられるようにする
    var useLocalJwt = builder.Configuration.GetValue<bool>("UseLocalJwt");

    if (useLocalJwt)
    {
        var jwtSection = builder.Configuration.GetSection("Jwt");
        var key = jwtSection.GetValue<string?>("Key") ?? throw new InvalidOperationException("Jwt:Key must be configured when UseLocalJwt is true");
        var issuer = jwtSection.GetValue<string?>("Issuer") ?? throw new InvalidOperationException("Jwt:Issuer must be configured when UseLocalJwt is true");
        var audience = jwtSection.GetValue<string?>("Audience") ?? throw new InvalidOperationException("Jwt:Audience must be configured when UseLocalJwt is true");

        var keyBytes = System.Text.Encoding.UTF8.GetBytes(key);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ValidateLifetime = true
                };
            });
    }
    else
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureEntraId"));
    }
    builder.Services.AddAuthorization();
}
else
{
    // 認証無効時：ダミーの認証スキームを登録して [Authorize] を無視
    builder.Services.AddAuthentication("NoAuth")
        .AddScheme<AuthenticationSchemeOptions, NoAuthHandler>("NoAuth", options => { });
    builder.Services.AddAuthorization();
}

// HttpClient for PokeAPI
builder.Services.AddHttpClient<PokeApiService>();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services
builder.Services.AddScoped<PokeApiService>();
builder.Services.AddScoped<PokemonSeedService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// EntityFrameworkCoreの設定
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Seed
if (args.Contains("--seed"))
{
    using var scope = app.Services.CreateScope();
    var seedService = scope.ServiceProvider.GetRequiredService<PokemonSeedService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    var seedType = args.Contains("--species") ? "species" :
                   args.Contains("--moves") ? "moves" : "all";

    int startId = 1;
    int endId = seedType == "moves" ? 165 : 151;

    // startとendの指定を確認
    var startIndex = Array.IndexOf(args, "--start");
    if (startIndex >= 0 && startIndex + 1 < args.Length)
    {
        int.TryParse(args[startIndex + 1], out startId);
    }

    var endIndex = Array.IndexOf(args, "--end");
    if (endIndex >= 0 && endIndex + 1 < args.Length)
    {
        int.TryParse(args[endIndex + 1], out endId);
    }

    logger.LogInformation("Starting seed process: {SeedType}", seedType);

    try
    {
        if (seedType == "species" || seedType == "all")
        {
            await seedService.SeedPokemonSpeciesAsync(startId, endId);
        }

        if (seedType == "moves" || seedType == "all")
        {
            await seedService.SeedMovesAsync(startId, endId);
        }

        logger.LogInformation("Seed process completed successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during seed process");
    }

    return;
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

// 認証が有効な場合のみミドルウェアを登録
if (isAuthenticationEnabled)
{
    app.UseAuthentication(); // 認証ミドルウェア
    app.UseAuthorization();  // 認可ミドルウェア
}

// app.UseHttpsRedirection();
app.MapControllers();

app.Run();
