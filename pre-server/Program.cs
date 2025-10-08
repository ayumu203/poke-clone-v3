using Microsoft.EntityFrameworkCore;
using server.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using server.Services;
using server.Interfaces;
using server.Factories;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("server.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

var builder = WebApplication.CreateBuilder(args);

// SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Azure Entra ID 認証
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
    {
        builder.Configuration.Bind("AzureAd", options);
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                await userService.EnsureUserExistsAsync(context);
            }
        };
    }, options => { builder.Configuration.Bind("AzureAd", options); });
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS設定
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

builder.Services.AddControllers();

builder.Services.AddSignalR();

// PokeApiSeeder の登録
builder.Services.AddHttpClient();
// builder.Services.AddScoped<PokeApiSeeder>();
builder.Services.AddScoped<IPokeApiExtractor, PokeApiExtractor>();
builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CpuPlayer と BattleManager の登録
builder.Services.AddSingleton<server.Models.Battles.Players.CpuPlayer>();
builder.Services.AddSingleton<server.Models.Battles.Services.BattleRoomManager>();

// PokemonFactory の登録
builder.Services.AddScoped<IPokemonFactory, PokemonFactory>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 開発環境ではHTTPSリダイレクトを無効化することを推奨
// app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<server.Hubs.BattleHub>("/battlehub");

app.Run();

public partial class Program { }