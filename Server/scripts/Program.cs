using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PokeApiDataFetcher;

class Program
{
    private const string BaseUrl = "https://pokeapi.co/api/v2";
    private const int RequestDelayMs = 100; // 100ms delay between requests
    private const int MaxPokemonId = 649; // Generation 5 ends at ID 649
    private const int MaxRetries = 3;
    private static readonly HttpClient httpClient = new();

    static async Task Main(string[] args)
    {
        Console.WriteLine("=".PadRight(60, '='));
        Console.WriteLine("PokeAPI Data Fetcher - Generations 1-5");
        Console.WriteLine("=".PadRight(60, '='));

        var outputDir = Path.Combine("..", "..", "Docs", "seeds");
        Directory.CreateDirectory(outputDir);

        // Fetch Pokemon data
        Console.WriteLine($"\nFetching Pokemon data (IDs 1-{MaxPokemonId})...");
        var pokemonList = new List<PokemonSpeciesDto>();
        var allMoveIds = new HashSet<int>();

        for (int pokemonId = 1; pokemonId <= MaxPokemonId; pokemonId++)
        {
            Console.Write($"\rProgress: {pokemonId}/{MaxPokemonId}");

            try
            {
                var pokemonData = await FetchPokemonDataAsync(pokemonId);
                if (pokemonData == null)
                {
                    Console.WriteLine($"\nWarning: Failed to fetch Pokemon ID {pokemonId}");
                    continue;
                }

                var speciesData = await FetchPokemonSpeciesDataAsync(pokemonId);
                if (speciesData != null)
                {
                    pokemonData.Name = speciesData.Value.JapaneseName ?? pokemonData.Name;
                    pokemonData.EvolveLevel = speciesData.Value.EvolveLevel;
                }

                foreach (var moveId in pokemonData.MoveIds)
                {
                    allMoveIds.Add(moveId);
                }

                pokemonList.Add(pokemonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError processing Pokemon ID {pokemonId}: {ex.Message}");
            }
        }

        Console.WriteLine($"\n\nSuccessfully fetched {pokemonList.Count} Pokemon");

        // Save Pokemon data
        var pokemonFile = Path.Combine(outputDir, "pokemons.json");
        await File.WriteAllTextAsync(pokemonFile, 
            JsonSerializer.Serialize(pokemonList, new JsonSerializerOptions 
            { 
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));
        Console.WriteLine($"Saved Pokemon data to {pokemonFile}");

        // Fetch Move data
        Console.WriteLine($"\nFetching Move data ({allMoveIds.Count} unique moves)...");
        var moveList = new List<MoveDto>();
        var sortedMoveIds = allMoveIds.OrderBy(x => x).ToList();

        for (int i = 0; i < sortedMoveIds.Count; i++)
        {
            Console.Write($"\rProgress: {i + 1}/{sortedMoveIds.Count}");

            try
            {
                var moveData = await FetchMoveDataAsync(sortedMoveIds[i]);
                if (moveData != null)
                {
                    moveList.Add(moveData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError processing Move ID {sortedMoveIds[i]}: {ex.Message}");
            }
        }

        Console.WriteLine($"\n\nSuccessfully fetched {moveList.Count} moves");

        // Save Move data
        var movesFile = Path.Combine(outputDir, "moves.json");
        await File.WriteAllTextAsync(movesFile,
            JsonSerializer.Serialize(moveList, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));
        Console.WriteLine($"Saved Move data to {movesFile}");

        Console.WriteLine("\n" + "=".PadRight(60, '='));
        Console.WriteLine("Data fetching completed successfully!");
        Console.WriteLine("=".PadRight(60, '='));
    }

    static async Task<T?> FetchWithRetryAsync<T>(string url) where T : class
    {
        for (int attempt = 0; attempt < MaxRetries; attempt++)
        {
            try
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                await Task.Delay(RequestDelayMs);
                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception ex)
            {
                if (attempt == MaxRetries - 1)
                {
                    Console.WriteLine($"\nError fetching {url}: {ex.Message}");
                    return null;
                }
                await Task.Delay(1000); // Wait before retry
            }
        }
        return null;
    }

    static async Task<PokemonSpeciesDto?> FetchPokemonDataAsync(int pokemonId)
    {
        var url = $"{BaseUrl}/pokemon/{pokemonId}";
        var data = await FetchWithRetryAsync<PokeApiPokemon>(url);
        if (data == null) return null;

        var dto = new PokemonSpeciesDto
        {
            PokemonSpeciesId = data.Id,
            Name = CapitalizeFirstLetter(data.Name),
            FrontImage = data.Sprites.FrontDefault ?? "",
            BackImage = data.Sprites.BackDefault ?? "",
            Type1 = CapitalizeFirstLetter(data.Types[0].Type.Name),
            Type2 = data.Types.Count > 1 ? CapitalizeFirstLetter(data.Types[1].Type.Name) : null,
            BaseHp = data.Stats.First(s => s.Stat.Name == "hp").BaseStat,
            BaseAttack = data.Stats.First(s => s.Stat.Name == "attack").BaseStat,
            BaseDefense = data.Stats.First(s => s.Stat.Name == "defense").BaseStat,
            BaseSpecialAttack = data.Stats.First(s => s.Stat.Name == "special-attack").BaseStat,
            BaseSpecialDefense = data.Stats.First(s => s.Stat.Name == "special-defense").BaseStat,
            BaseSpeed = data.Stats.First(s => s.Stat.Name == "speed").BaseStat,
            EvolveLevel = 99,
            MoveIds = data.Moves.Take(20).Select(m => int.Parse(m.Move.Url.Split('/')[^2])).ToList()
        };

        return dto;
    }

    static async Task<(string? JapaneseName, int EvolveLevel)?> FetchPokemonSpeciesDataAsync(int pokemonId)
    {
        var url = $"{BaseUrl}/pokemon-species/{pokemonId}";
        var data = await FetchWithRetryAsync<PokeApiPokemonSpecies>(url);
        if (data == null) return null;

        var japaneseName = data.Names.FirstOrDefault(n => n.Language.Name == "ja")?.Value;
        var evolveLevel = 99;

        if (data.EvolutionChain?.Url != null)
        {
            var evolutionData = await FetchWithRetryAsync<PokeApiEvolutionChain>(data.EvolutionChain.Url);
            if (evolutionData != null)
            {
                evolveLevel = ParseEvolutionLevel(evolutionData.Chain, pokemonId);
            }
        }

        return (japaneseName, evolveLevel);
    }

    static int ParseEvolutionLevel(ChainLink chain, int targetId)
    {
        foreach (var evolution in chain.EvolvesTo)
        {
            var evolvedId = int.Parse(evolution.Species.Url.Split('/')[^2]);
            if (evolvedId == targetId)
            {
                var levelUpDetail = evolution.EvolutionDetails.FirstOrDefault(d => d.Trigger.Name == "level-up");
                return levelUpDetail?.MinLevel ?? 99;
            }

            var result = ParseEvolutionLevel(evolution, targetId);
            if (result != 99) return result;
        }

        return 99;
    }

    static async Task<MoveDto?> FetchMoveDataAsync(int moveId)
    {
        var url = $"{BaseUrl}/move/{moveId}";
        var data = await FetchWithRetryAsync<PokeApiMove>(url);
        if (data == null) return null;

        var japaneseName = data.Names.FirstOrDefault(n => n.Language.Name == "ja")?.Value ?? data.Name;

        var dto = new MoveDto
        {
            MoveId = data.Id,
            Name = japaneseName,
            Type = CapitalizeFirstLetter(data.Type.Name),
            Power = data.Power ?? 0,
            Accuracy = data.Accuracy ?? 100,
            Pp = data.Pp ?? 0,
            Priority = data.Priority ?? 0,
            DamageClass = CapitalizeFirstLetter(data.DamageClass.Name),
            Category = data.Meta?.Category?.Name ?? "damage", // Use meta category if available
            Ailment = data.Meta?.Ailment?.Name ?? "none",
            AilmentChance = data.Meta?.AilmentChance ?? 0,
            Healing = data.Meta?.Healing ?? 0,
            Drain = data.Meta?.Drain ?? 0,
            CritRate = data.Meta?.CritRate ?? 0,
            StatChanges = data.StatChanges.Select(sc => new StatChangeDto
            {
                Stat = sc.Stat.Name,
                Change = sc.Change
            }).ToList()
        };

        return dto;
    }

    static string CapitalizeFirstLetter(string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        return char.ToUpper(str[0]) + str.Substring(1);
    }
}

// DTOs for output
public class PokemonSpeciesDto
{
    [JsonPropertyName("pokemonSpeciesId")]
    public int PokemonSpeciesId { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("frontImage")]
    public string FrontImage { get; set; } = string.Empty;
    
    [JsonPropertyName("backImage")]
    public string BackImage { get; set; } = string.Empty;
    
    [JsonPropertyName("type1")]
    public string Type1 { get; set; } = string.Empty;
    
    [JsonPropertyName("type2")]
    public string? Type2 { get; set; }
    
    [JsonPropertyName("evolveLevel")]
    public int EvolveLevel { get; set; }
    
    [JsonPropertyName("baseHp")]
    public int BaseHp { get; set; }
    
    [JsonPropertyName("baseAttack")]
    public int BaseAttack { get; set; }
    
    [JsonPropertyName("baseDefense")]
    public int BaseDefense { get; set; }
    
    [JsonPropertyName("baseSpecialAttack")]
    public int BaseSpecialAttack { get; set; }
    
    [JsonPropertyName("baseSpecialDefense")]
    public int BaseSpecialDefense { get; set; }
    
    [JsonPropertyName("baseSpeed")]
    public int BaseSpeed { get; set; }
    
    [JsonPropertyName("moveIds")]
    public List<int> MoveIds { get; set; } = new();
}

public class MoveDto
{
    [JsonPropertyName("moveId")]
    public int MoveId { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("power")]
    public int Power { get; set; }
    
    [JsonPropertyName("accuracy")]
    public int Accuracy { get; set; }
    
    [JsonPropertyName("pp")]
    public int Pp { get; set; }
    
    [JsonPropertyName("priority")]
    public int Priority { get; set; }
    
    [JsonPropertyName("damageClass")]
    public string DamageClass { get; set; } = string.Empty;
    
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("ailment")]
    public string Ailment { get; set; } = string.Empty;

    [JsonPropertyName("ailmentChance")]
    public int AilmentChance { get; set; }

    [JsonPropertyName("healing")]
    public int Healing { get; set; }

    [JsonPropertyName("drain")]
    public int Drain { get; set; }

    [JsonPropertyName("critRate")]
    public int CritRate { get; set; }

    [JsonPropertyName("statChanges")]
    public List<StatChangeDto> StatChanges { get; set; } = new();
}

public class StatChangeDto
{
    [JsonPropertyName("stat")]
    public string Stat { get; set; } = string.Empty;

    [JsonPropertyName("change")]
    public int Change { get; set; }
}

// PokeAPI response models
public class PokeApiPokemon
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("sprites")]
    public Sprites Sprites { get; set; } = new();
    
    [JsonPropertyName("types")]
    public List<TypeSlot> Types { get; set; } = new();
    
    [JsonPropertyName("stats")]
    public List<StatSlot> Stats { get; set; } = new();
    
    [JsonPropertyName("moves")]
    public List<MoveSlot> Moves { get; set; } = new();
}

public class Sprites
{
    [JsonPropertyName("front_default")]
    public string? FrontDefault { get; set; }
    
    [JsonPropertyName("back_default")]
    public string? BackDefault { get; set; }
}

public class TypeSlot
{
    [JsonPropertyName("type")]
    public NamedResource Type { get; set; } = new();
}

public class StatSlot
{
    [JsonPropertyName("base_stat")]
    public int BaseStat { get; set; }
    
    [JsonPropertyName("stat")]
    public NamedResource Stat { get; set; } = new();
}

public class MoveSlot
{
    [JsonPropertyName("move")]
    public NamedResource Move { get; set; } = new();
}

public class PokeApiPokemonSpecies
{
    [JsonPropertyName("names")]
    public List<Name> Names { get; set; } = new();
    
    [JsonPropertyName("evolution_chain")]
    public EvolutionChainRef? EvolutionChain { get; set; }
}

public class Name
{
    [JsonPropertyName("name")]
    public string Value { get; set; } = string.Empty;
    
    [JsonPropertyName("language")]
    public NamedResource Language { get; set; } = new();
}

public class EvolutionChainRef
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

public class PokeApiEvolutionChain
{
    [JsonPropertyName("chain")]
    public ChainLink Chain { get; set; } = new();
}

public class ChainLink
{
    [JsonPropertyName("species")]
    public NamedResource Species { get; set; } = new();
    
    [JsonPropertyName("evolves_to")]
    public List<ChainLink> EvolvesTo { get; set; } = new();
    
    [JsonPropertyName("evolution_details")]
    public List<EvolutionDetail> EvolutionDetails { get; set; } = new();
}

public class EvolutionDetail
{
    [JsonPropertyName("min_level")]
    public int? MinLevel { get; set; }
    
    [JsonPropertyName("trigger")]
    public NamedResource Trigger { get; set; } = new();
}

public class PokeApiMove
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("names")]
    public List<Name> Names { get; set; } = new();
    
    [JsonPropertyName("type")]
    public NamedResource Type { get; set; } = new();
    
    [JsonPropertyName("power")]
    public int? Power { get; set; }
    
    [JsonPropertyName("accuracy")]
    public int? Accuracy { get; set; }
    
    [JsonPropertyName("pp")]
    public int? Pp { get; set; }
    
    [JsonPropertyName("priority")]
    public int? Priority { get; set; }
    
    [JsonPropertyName("damage_class")]
    public NamedResource DamageClass { get; set; } = new();

    [JsonPropertyName("meta")]
    public MoveMeta? Meta { get; set; }

    [JsonPropertyName("stat_changes")]
    public List<MoveStatChange> StatChanges { get; set; } = new();
}

public class MoveMeta
{
    [JsonPropertyName("ailment")]
    public NamedResource Ailment { get; set; } = new();

    [JsonPropertyName("category")]
    public NamedResource Category { get; set; } = new();

    [JsonPropertyName("min_hits")]
    public int? MinHits { get; set; }

    [JsonPropertyName("max_hits")]
    public int? MaxHits { get; set; }

    [JsonPropertyName("min_turns")]
    public int? MinTurns { get; set; }

    [JsonPropertyName("max_turns")]
    public int? MaxTurns { get; set; }

    [JsonPropertyName("drain")]
    public int Drain { get; set; }

    [JsonPropertyName("healing")]
    public int Healing { get; set; }

    [JsonPropertyName("crit_rate")]
    public int CritRate { get; set; }

    [JsonPropertyName("ailment_chance")]
    public int AilmentChance { get; set; }

    [JsonPropertyName("flinch_chance")]
    public int FlinchChance { get; set; }

    [JsonPropertyName("stat_chance")]
    public int StatChance { get; set; }
}

public class MoveStatChange
{
    [JsonPropertyName("change")]
    public int Change { get; set; }

    [JsonPropertyName("stat")]
    public NamedResource Stat { get; set; } = new();
}

public class NamedResource
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}
