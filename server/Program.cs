using Microsoft.EntityFrameworkCore;
using server.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using server.Services;
using server.Interfaces;
using server.Factories;

var builder = WebApplication.CreateBuilder(args);

// SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 認証
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

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
builder.Services.AddSingleton<server.Models.Battles.CpuPlayer>();
builder.Services.AddSingleton<server.Models.Battles.BattleRoomManager>();

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
