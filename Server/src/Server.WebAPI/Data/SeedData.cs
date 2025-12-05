using Microsoft.EntityFrameworkCore;
using Server.Domain.Entities;
using Server.Domain.Enums;
using Server.Infrastructure.Data;
using System.Text.Json;

namespace Server.WebAPI.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // Apply migrations
        await context.Database.MigrateAsync();
        
        // Seed Moves
        if (!context.Moves.Any())
        {
            var movesJson = await File.ReadAllTextAsync("/seeds/moves.json");
            var moves = JsonSerializer.Deserialize<List<MoveDto>>(movesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            if (moves != null)
            {
                foreach (var dto in moves)
                {
                    context.Moves.Add(new Move
                    {
                        MoveId = dto.MoveId,
                        Name = dto.Name,
                        Type = Enum.Parse<PokemonType>(dto.Type),
                        Power = dto.Power,
                        Accuracy = dto.Accuracy,
                        Pp = dto.Pp,
                        Priority = dto.Priority,
                        DamageClass = Enum.Parse<DamageClass>(dto.DamageClass),
                        Category = Enum.Parse<Category>(dto.Category),
                        Target = dto.Target ?? "selected-pokemon",
                        StatChance = dto.StatChance,
                        Ailment = Enum.TryParse<Ailment>(dto.Ailment, true, out var ailment) ? ailment : Ailment.None,
                        AilmentChance = dto.AilmentChance,
                        Healing = dto.Healing,
                        Drain = dto.Drain,
                        CritRate = dto.CritRate,
                        StatChanges = dto.StatChanges?.Select(sc => new StatChange
                        {
                            Stat = Enum.Parse<PokemonStat>(sc.Stat.Replace("-", ""), true),
                            Change = sc.Change
                        }).ToList() ?? new List<StatChange>()
                    });
                }
                await context.SaveChangesAsync();
            }
        }
        
        // Seed PokemonSpecies with MoveList
        if (!context.PokemonSpecies.Any())
        {
            var pokemonJson = await File.ReadAllTextAsync("/seeds/pokemons.json");
            var pokemonList = JsonSerializer.Deserialize<List<PokemonSpeciesDto>>(pokemonJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            if (pokemonList != null)
            {
                foreach (var dto in pokemonList)
                {
                    var species = new PokemonSpecies
                    {
                        PokemonSpeciesId = dto.PokemonSpeciesId,
                        Name = dto.Name,
                        FrontImage = dto.FrontImage,
                        BackImage = dto.BackImage,
                        Type1 = Enum.Parse<PokemonType>(dto.Type1),
                        Type2 = dto.Type2 != null ? Enum.Parse<PokemonType>(dto.Type2) : null,
                        EvolveLevel = dto.EvolveLevel,
                        BaseHp = dto.BaseHp,
                        BaseAttack = dto.BaseAttack,
                        BaseDefence = dto.BaseDefense,
                        BaseSpecialAttack = dto.BaseSpecialAttack,
                        BaseSpecialDefence = dto.BaseSpecialDefense,
                        BaseSpeed = dto.BaseSpeed,
                        MoveList = new List<Move>()
                    };
                    
                    // Add moves to MoveList
                    if (dto.MoveIds != null)
                    {
                        foreach (var moveId in dto.MoveIds)
                        {
                            var move = await context.Moves.FindAsync(moveId);
                            if (move != null)
                            {
                                species.MoveList.Add(move);
                            }
                        }
                    }
                    
                    context.PokemonSpecies.Add(species);
                }
                await context.SaveChangesAsync();
            }
        }
    }
    
    private class MoveDto
    {
        public int MoveId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Power { get; set; }
        public int Accuracy { get; set; }
        public int Pp { get; set; }
        public int Priority { get; set; }
        public string DamageClass { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Ailment { get; set; } = string.Empty;
        public int AilmentChance { get; set; }
        public int Healing { get; set; }
        public int Drain { get; set; }
        public int CritRate { get; set; }
        public List<StatChangeDto>? StatChanges { get; set; }
        public string? Target { get; set; }
        public int StatChance { get; set; }
    }

    private class StatChangeDto
    {
        public string Stat { get; set; } = string.Empty;
        public int Change { get; set; }
    }
    
    private class PokemonSpeciesDto
    {
        public int PokemonSpeciesId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FrontImage { get; set; } = string.Empty;
        public string BackImage { get; set; } = string.Empty;
        public string Type1 { get; set; } = string.Empty;
        public string? Type2 { get; set; }
        public int EvolveLevel { get; set; }
        public int BaseHp { get; set; }
        public int BaseAttack { get; set; }
        public int BaseDefense { get; set; }
        public int BaseSpecialAttack { get; set; }
        public int BaseSpecialDefense { get; set; }
        public int BaseSpeed { get; set; }
        public List<int>? MoveIds { get; set; }
    }
}
