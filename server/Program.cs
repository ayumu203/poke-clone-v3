using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using server.Data;
using server.Services;
using server.Helpers;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

static bool ConfigureAuthentication(WebApplicationBuilder builder)
{
    var isAuthenticationEnabled = builder.Configuration.GetValue<bool>("IsAuthenticationEnabled", true);
    JwtHelper.Initialize(builder.Configuration);

    if (isAuthenticationEnabled)
    {
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
        builder.Services.AddAuthentication("NoAuth")
            .AddScheme<AuthenticationSchemeOptions, server.Services.NoAuthHandler>("NoAuth", options => { });
        builder.Services.AddAuthorization();
    }

    return isAuthenticationEnabled;
}

var isAuthenticationEnabled = ConfigureAuthentication(builder);

builder.Services.AddHttpClient<PokeApiService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<PokeApiService>();
builder.Services.AddScoped<PokemonSeedService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error applying database migrations");
        throw;
    }
}

if (args.Contains("--seed"))
{
    using var scope = app.Services.CreateScope();
    var seedService = scope.ServiceProvider.GetRequiredService<PokemonSeedService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    var seedType = args.Contains("--species") ? "species" :
                   args.Contains("--moves") ? "moves" : "all";

    int startId = 1;
    int endId = seedType == "moves" ? 165 : 151;

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
        logger.LogInformation("Dumping JSON files for seed type: {SeedType}", seedType);
            if (seedType == "species" || seedType == "all")
        {
            await seedService.DumpPokemonJsonAsync(startId, endId, "Data/pokemons");
            await seedService.SeedPokemonFromJsonFolderAsync("Data/pokemons");
        }

        if (seedType == "moves" || seedType == "all")
        {
            await seedService.DumpMovesJsonAsync(startId, endId, "Data/moves");
            await seedService.SeedMovesFromJsonFolderAsync("Data/moves");
        }

        logger.LogInformation("Seed process completed successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during seed process");
    }

    return;
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

if (isAuthenticationEnabled)
{
    app.UseAuthentication();
    app.UseAuthorization();
}

app.MapControllers();

app.Run();
