using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Services;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });





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

// テスト環境以外でDbContextを登録
if (builder.Environment.EnvironmentName != "Test")
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

var app = builder.Build();

// テスト環境ではマイグレーションをスキップ
if (app.Environment.EnvironmentName != "Test")
{
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



app.MapControllers();

app.Run();

// テスト用に Program クラスを公開
public partial class Program { }
