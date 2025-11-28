using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Server.Infrastructure.Data;
using Server.Domain.Repositories;
using Server.Infrastructure.Repositories;
using Server.Application.Services;
using Server.Domain.Services;
using Server.WebAPI.Hubs;
using Server.WebAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy (policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// JWT Bearer Authentication (Development)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

// SignalR
builder.Services.AddSignalR();

// DbContext Configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis Configuration
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnectionString!));

// Repository Registration
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IPokemonSpeciesRepository, PokemonSpeciesRepository>();
builder.Services.AddScoped<IMoveRepository, MoveRepository>();
builder.Services.AddScoped<IBattleRepository, RedisBattleRepository>();

// Domain Services Registration
builder.Services.AddSingleton<ITypeEffectivenessManager, TypeEffectivenessManager>();
builder.Services.AddSingleton<IStatCalculator, StatCalculator>();
builder.Services.AddScoped<IDamageCalculator, DamageCalculator>();

// Application Services Registration
builder.Services.AddScoped<IBattleService, BattleService>();

var app = builder.Build();

// Initialize SeedData
using (var scope = app.Services.CreateScope())
{
    await SeedData.Initialize(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<BattleHub>("/battlehub");

app.Run();
