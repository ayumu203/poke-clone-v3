using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Server.Infrastructure.Data;
using Server.Domain.Repositories;
using Server.Infrastructure.Repositories;
using Server.Application.Services;
using Server.Domain.Services;
using Server.Infrastructure.Services;
using Server.WebAPI.Hubs;
using Server.WebAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:8080", "http://localhost:8000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
        
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["access_token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });

// SignalR
builder.Services.AddSignalR();

// DbContext Configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Server.WebAPI")));

// Redis Configuration
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnectionString!));

// Repository Registration
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IPokemonSpeciesRepository, PokemonSpeciesRepository>();
builder.Services.AddScoped<IMoveRepository, MoveRepository>();
builder.Services.AddScoped<IBattleRepository, RedisBattleRepository>();
builder.Services.AddScoped<IPokemonRepository, PokemonRepository>();
builder.Services.AddScoped<IPlayerPartyRepository, PlayerPartyRepository>();

// Domain Services Registration
builder.Services.AddSingleton<ITypeEffectivenessManager, TypeEffectivenessManager>();
builder.Services.AddSingleton<IStatCalculator, StatCalculator>();
builder.Services.AddScoped<IDamageCalculator, DamageCalculator>();
builder.Services.AddScoped<IExpCalculator, ExpCalculator>();
builder.Services.AddScoped<IEvolutionService, EvolutionService>();

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

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<BattleHub>("/battlehub");

app.Run();
