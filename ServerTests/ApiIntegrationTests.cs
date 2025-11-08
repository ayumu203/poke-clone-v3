using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using server.Data;
using server.Helpers;
using server.Models.Core;
using server.Models.DTOs;
using server.Models.Enums;
using Xunit;

namespace ServerTests;

public class ApiIntegrationTests : IDisposable
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly string _testPlayerId = "test-player-id";
    private string? _authToken;

    public ApiIntegrationTests()
    {
        _factory = new CustomWebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    private void SetupAuthentication()
    {
        // テスト用JWTトークンを直接生成
        var key = "test-secret-key-for-jwt-authentication-minimum-32-characters";
        var keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
        var securityKey = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, _testPlayerId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: "test-issuer",
            audience: "test-audience",
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        var handler = new JwtSecurityTokenHandler();
        _authToken = handler.WriteToken(token);
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
    public async Task Test1_CreatePlayer_ShouldReturnCreatedPlayer()
    {
        // Arrange
        SetupAuthentication();
        var createDto = await LoadTestDataAsync<CreatePlayerDto>("player.json");

        // Act
        var response = await _client.PostAsJsonAsync("/api/players", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var result = await response.Content.ReadFromJsonAsync<PlayerDto>();
        result.Should().NotBeNull();
        result!.PlayerId.Should().Be(_testPlayerId);
        result.Name.Should().Be("テストユーザー");
        result.IconUrl.Should().Be("https://example.com/icon.png");
    }

    [Fact]
    public async Task Test2_GetPlayer_ShouldReturnPlayerInfo()
    {
        // Arrange
        SetupAuthentication();
        
        // プレイヤーを作成
        var createDto = await LoadTestDataAsync<CreatePlayerDto>("player.json");
        await _client.PostAsJsonAsync("/api/players", createDto);

        // Act
        var response = await _client.GetAsync($"/api/players/{_testPlayerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<PlayerDto>();
        result.Should().NotBeNull();
        result!.PlayerId.Should().Be(_testPlayerId);
        result.Name.Should().Be("テストユーザー");
        result.IconUrl.Should().Be("https://example.com/icon.png");

        // 期待されるJSONとの比較
        var expectedPlayer = await LoadTestDataAsync<PlayerDto>("expected_player.json");
        result.PlayerId.Should().Be(expectedPlayer.PlayerId);
        result.Name.Should().Be(expectedPlayer.Name);
        result.IconUrl.Should().Be(expectedPlayer.IconUrl);
    }

    [Fact]
    public async Task Test3_AddPokemonToParty_ShouldReturnCreatedPokemon()
    {
        // Arrange
        SetupAuthentication();
        
        // プレイヤーを作成
        var createPlayerDto = await LoadTestDataAsync<CreatePlayerDto>("player.json");
        await _client.PostAsJsonAsync("/api/players", createPlayerDto);

        // ポケモン種族データをシード
        await SeedTestPokemonSpecies();

        var createPokemonDto = await LoadTestDataAsync<CreatePokemonDto>("pokemon.json");

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/players/{_testPlayerId}/party", 
            createPokemonDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var result = await response.Content.ReadFromJsonAsync<PokemonDto>();
        result.Should().NotBeNull();
        result!.Level.Should().Be(5);
        result.Species.PokemonSpeciesId.Should().Be(1);
        result.Species.Name.Should().Be("フシギダネ");
    }

    [Fact]
    public async Task Test4_GetParty_ShouldReturnPartyList()
    {
        // Arrange
        SetupAuthentication();
        
        // プレイヤーとポケモンを作成
        var createPlayerDto = await LoadTestDataAsync<CreatePlayerDto>("player.json");
        await _client.PostAsJsonAsync("/api/players", createPlayerDto);

        await SeedTestPokemonSpecies();

        var createPokemonDto = await LoadTestDataAsync<CreatePokemonDto>("pokemon.json");
        await _client.PostAsJsonAsync(
            $"/api/players/{_testPlayerId}/party", 
            createPokemonDto);

        // Act
        var response = await _client.GetAsync($"/api/players/{_testPlayerId}/party");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<PlayerPartyListDto>();
        result.Should().NotBeNull();
        result!.Pokemons.Should().HaveCount(1);
        result.Pokemons[0].Level.Should().Be(5);
        result.Pokemons[0].Species.PokemonSpeciesId.Should().Be(1);
        result.Pokemons[0].Species.Name.Should().Be("フシギダネ");

        // JSON比較
        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        doc.RootElement.GetProperty("pokemons").GetArrayLength().Should().Be(1);
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
                Type1 = Enum.Parse<server.Models.Enums.Type>(speciesData.Type1),
                Type2 = string.IsNullOrEmpty(speciesData.Type2) ? null : Enum.Parse<server.Models.Enums.Type>(speciesData.Type2),
                EvolveLevel = speciesData.EvolveLevel ?? 0,
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

    // JSONデシリアライズ用のヘルパークラス
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
