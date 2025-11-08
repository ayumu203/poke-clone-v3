using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using server.Data;
using server.Helpers;
using server.Models.Core;
using server.Models.DTOs;
using server.Models.Enums;
using System.Net.Http.Headers;
using System.Text.Json;
using VerifyXunit;

namespace server.Tests;

[UsesVerify]
public class ApiSnapshotTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly string _testPlayerId = "test-player-id";
    private string? _authToken;

    public ApiSnapshotTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // テスト用にインメモリDBに置き換え
                services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"SnapshotTestDb_{Guid.NewGuid()}");
                });

                // DBの初期化
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();
            });
        });

        _client = _factory.CreateClient();
    }

    private void SetupAuthentication()
    {
        _authToken = JwtHelper.GenerateLocalJwt(_testPlayerId);
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _authToken);
    }

    private async Task<T> LoadTestDataAsync<T>(string fileName)
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", fileName);
        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        })!;
    }

    [Fact]
    public async Task CreatePlayer_ShouldMatchSnapshot()
    {
        // Arrange
        SetupAuthentication();
        var createDto = await LoadTestDataAsync<CreatePlayerDto>("player.json");

        // Act
        var response = await _client.PostAsJsonAsync("/api/players", createDto);
        var result = await response.Content.ReadFromJsonAsync<PlayerDto>();

        // Assert - スナップショットと比較
        await Verify(result)
            .UseDirectory("Snapshots")
            .UseMethodName("CreatePlayer");
    }

    [Fact]
    public async Task GetPlayer_ShouldMatchSnapshot()
    {
        // Arrange
        SetupAuthentication();
        var createDto = await LoadTestDataAsync<CreatePlayerDto>("player.json");
        await _client.PostAsJsonAsync("/api/players", createDto);

        // Act
        var response = await _client.GetAsync($"/api/players/{_testPlayerId}");
        var result = await response.Content.ReadFromJsonAsync<PlayerDto>();

        // Assert
        await Verify(result)
            .UseDirectory("Snapshots")
            .UseMethodName("GetPlayer");
    }

    [Fact]
    public async Task GetParty_ShouldMatchSnapshot()
    {
        // Arrange
        SetupAuthentication();
        
        // プレイヤーを作成
        var createPlayerDto = await LoadTestDataAsync<CreatePlayerDto>("player.json");
        await _client.PostAsJsonAsync("/api/players", createPlayerDto);

        // ポケモン種族データをシード
        await SeedTestPokemonSpecies();

        // ポケモンを追加
        var createPokemonDto = await LoadTestDataAsync<CreatePokemonDto>("pokemon.json");
        await _client.PostAsJsonAsync($"/api/players/{_testPlayerId}/party", createPokemonDto);

        // Act
        var response = await _client.GetAsync($"/api/players/{_testPlayerId}/party");
        var result = await response.Content.ReadFromJsonAsync<PlayerPartyListDto>();

        // Assert
        await Verify(result)
            .UseDirectory("Snapshots")
            .UseMethodName("GetParty");
    }

    private async Task SeedTestPokemonSpecies()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        if (!await db.PokemonSpecies.AnyAsync())
        {
            var speciesData = await LoadTestDataAsync<PokemonSpeciesData>("pokemon_species.json");
            
            db.PokemonSpecies.Add(new PokemonSpecies
            {
                PokemonSpeciesId = speciesData.PokemonSpeciesId,
                Name = speciesData.Name,
                FrontImage = speciesData.FrontImage,
                BackImage = speciesData.BackImage,
                Type1 = Enum.Parse<Models.Enums.Type>(speciesData.Type1),
                Type2 = string.IsNullOrEmpty(speciesData.Type2) ? null : Enum.Parse<Models.Enums.Type>(speciesData.Type2),
                EvolveLevel = speciesData.EvolveLevel,
                BaseHP = speciesData.BaseHP,
                BaseAttack = speciesData.BaseAttack,
                BaseDefense = speciesData.BaseDefense,
                BaseSpecialAttack = speciesData.BaseSpecialAttack,
                BaseSpecialDefense = speciesData.BaseSpecialDefense,
                BaseSpeed = speciesData.BaseSpeed
            });
            await db.SaveChangesAsync();
        }
    }

    private class PokemonSpeciesData
    {
        public int PokemonSpeciesId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FrontImage { get; set; } = string.Empty;
        public string BackImage { get; set; } = string.Empty;
        public string Type1 { get; set; } = string.Empty;
        public string? Type2 { get; set; }
        public int? EvolveLevel { get; set; }
        public int BaseHP { get; set; }
        public int BaseAttack { get; set; }
        public int BaseDefense { get; set; }
        public int BaseSpecialAttack { get; set; }
        public int BaseSpecialDefense { get; set; }
        public int BaseSpeed { get; set; }
    }
}
