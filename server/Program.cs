using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Services;

var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
