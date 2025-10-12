using System.Text.Json;
using System.IO;
using server.Models.Core;
using server.Models.Enums;

namespace server.Services
{
    public class PokeApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PokeApiService> _logger;
        private const string BaseUrl = "https://pokeapi.co/api/v2";

        public PokeApiService(HttpClient httpClient, ILogger<PokeApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<PokemonSpecies?> GetPokemonSpeciesAsync(int id)
        {
            try
            {
                // ポケモンデータ取得
                var pokemonResponse = await _httpClient.GetStringAsync($"{BaseUrl}/pokemon/{id}");
                var pokemonData = JsonDocument.Parse(pokemonResponse);

                // 種データから日本語名を取得
                var speciesUrl = pokemonData.RootElement.GetProperty("species").GetProperty("url").GetString();
                var speciesResponse = await _httpClient.GetStringAsync(speciesUrl);
                var speciesData = JsonDocument.Parse(speciesResponse);

                var japaneseName = GetJapaneseName(speciesData, "names");

                var species = new PokemonSpecies
                {
                    PokemonSpeciesId = id,
                    Name = japaneseName,
                    FrontImage = pokemonData.RootElement
                        .GetProperty("sprites")
                        .GetProperty("front_default")
                        .GetString() ?? "",
                    BackImage = pokemonData.RootElement
                        .GetProperty("sprites")
                        .GetProperty("back_default")
                        .GetString() ?? "",
                    EvolveLevel = 0, // 進化レベルは別途設定が必要
                    BaseHP = 0,
                    BaseAttack = 0,
                    BaseDefense = 0,
                    BaseSpecialAttack = 0,
                    BaseSpecialDefense = 0,
                    BaseSpeed = 0
                };

                // タイプ情報を取得
                var types = new List<server.Models.Enums.Type>();
                foreach (var typeElement in pokemonData.RootElement.GetProperty("types").EnumerateArray())
                {
                    var typeName = typeElement.GetProperty("type").GetProperty("name").GetString() ?? "";
                    var pokemonType = MapTypeFromString(typeName);
                    types.Add(pokemonType);
                }
                species.Type1 = types.Count > 0 ? types[0] : server.Models.Enums.Type.Normal;
                species.Type2 = types.Count > 1 ? types[1] : null;

                // ステータス情報を取得
                foreach (var statElement in pokemonData.RootElement.GetProperty("stats").EnumerateArray())
                {
                    var statName = statElement.GetProperty("stat").GetProperty("name").GetString();
                    var baseStat = statElement.GetProperty("base_stat").GetInt32();

                    switch (statName)
                    {
                        case "hp":
                            species.BaseHP = baseStat;
                            break;
                        case "attack":
                            species.BaseAttack = baseStat;
                            break;
                        case "defense":
                            species.BaseDefense = baseStat;
                            break;
                        case "special-attack":
                            species.BaseSpecialAttack = baseStat;
                            break;
                        case "special-defense":
                            species.BaseSpecialDefense = baseStat;
                            break;
                        case "speed":
                            species.BaseSpeed = baseStat;
                            break;
                    }
                }

                return species;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pokemon species {Id}", id);
                return null;
            }
        }

        public async Task<Move?> GetMoveAsync(int id)
        {
            try
            {
                var moveResponse = await _httpClient.GetStringAsync($"{BaseUrl}/move/{id}");
                var moveData = JsonDocument.Parse(moveResponse);

                var japaneseName = GetJapaneseName(moveData, "names");
                var typeName = moveData.RootElement.GetProperty("type").GetProperty("name").GetString() ?? "";
                var damageClassName = moveData.RootElement.GetProperty("damage_class").GetProperty("name").GetString() ?? "";

                var move = new Move
                {
                    MoveId = id,
                    Name = japaneseName,
                    Type = MapTypeFromString(typeName),
                    Category = MapCategoryFromMeta(moveData),
                    DamageClass = MapDamageClassFromString(damageClassName),
                    Power = moveData.RootElement.TryGetProperty("power", out var powerProp) && powerProp.ValueKind != JsonValueKind.Null 
                        ? powerProp.GetInt32() 
                        : 0,
                    Accuracy = moveData.RootElement.TryGetProperty("accuracy", out var accuracyProp) && accuracyProp.ValueKind != JsonValueKind.Null 
                        ? accuracyProp.GetInt32() 
                        : 100,
                    PP = (moveData.RootElement.TryGetProperty("pp", out var ppProp) && ppProp.ValueKind != JsonValueKind.Null)
                        ? ppProp.GetInt32()
                        : 0,
                    Priority = (moveData.RootElement.TryGetProperty("priority", out var priorityProp) && priorityProp.ValueKind != JsonValueKind.Null)
                        ? priorityProp.GetInt32()
                        : 0,
                    Rank = new Rank(),
                    StatTarget = "",
                    StatChance = 0,
                    Ailment = Ailment.None,
                    AilmentChance = 0,
                    Healing = 0,
                    Drain = 0
                };

                // メタ情報から追加効果を取得
                if (moveData.RootElement.TryGetProperty("meta", out var meta))
                {
                    // 状態異常
                    if (meta.TryGetProperty("ailment", out var ailmentProp))
                    {
                        var ailmentName = ailmentProp.GetProperty("name").GetString() ?? "";
                        move.Ailment = MapAilmentFromString(ailmentName);
                    }

                    // 状態異常の発生確率
                    if (meta.TryGetProperty("ailment_chance", out var ailmentChanceProp) && ailmentChanceProp.ValueKind != JsonValueKind.Null)
                    {
                        move.AilmentChance = ailmentChanceProp.GetInt32();
                    }

                    // ドレイン
                    if (meta.TryGetProperty("drain", out var drainProp) && drainProp.ValueKind != JsonValueKind.Null)
                    {
                        move.Drain = drainProp.GetInt32();
                    }

                    // 回復
                    if (meta.TryGetProperty("healing", out var healingProp) && healingProp.ValueKind != JsonValueKind.Null)
                    {
                        move.Healing = healingProp.GetInt32();
                    }
                }

                // ステータス変化の情報を取得
                if (moveData.RootElement.TryGetProperty("stat_changes", out var statChanges) && statChanges.GetArrayLength() > 0)
                {
                    var firstStatChange = statChanges[0];
                    var statName = firstStatChange.GetProperty("stat").GetProperty("name").GetString() ?? "";
                    var change = 0;
                    if (firstStatChange.TryGetProperty("change", out var changeProp) && changeProp.ValueKind != JsonValueKind.Null)
                    {
                        change = changeProp.GetInt32();
                    }

                    move.StatTarget = MapStatName(statName);
                    move.Rank = CreateRankFromStatChange(statName, change);
                    
                    // ステータス変化の確率（effect_chancesから取得）
                    if (moveData.RootElement.TryGetProperty("effect_chance", out var effectChance))
                    {
                        move.StatChance = effectChance.GetInt32();
                    }
                    else
                    {
                        move.StatChance = 100; // デフォルトで100%
                    }
                }

                return move;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching move {Id}", id);
                return null;
            }
        }

        // --- 新規追加: API レスポンスをローカル JSON として保存 ---
        public async Task SavePokemonJsonAsync(int id, string folderPath)
        {
            try
            {
                Directory.CreateDirectory(folderPath);

                var pokemonResponse = await _httpClient.GetStringAsync($"{BaseUrl}/pokemon/{id}");
                var pokemonDoc = JsonDocument.Parse(pokemonResponse);
                var speciesUrl = pokemonDoc.RootElement.GetProperty("species").GetProperty("url").GetString();

                string speciesResponse = "{}";
                if (!string.IsNullOrEmpty(speciesUrl))
                {
                    speciesResponse = await _httpClient.GetStringAsync(speciesUrl);
                }

                using var combined = new MemoryStream();
                using (var writer = new Utf8JsonWriter(combined))
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("pokemon");
                    JsonDocument.Parse(pokemonResponse).RootElement.WriteTo(writer);
                    writer.WritePropertyName("species");
                    JsonDocument.Parse(speciesResponse).RootElement.WriteTo(writer);
                    writer.WriteEndObject();
                }

                var filePath = Path.Combine(folderPath, $"pokemon_{id:D3}.json");
                await File.WriteAllBytesAsync(filePath, combined.ToArray());
                _logger.LogInformation("Saved pokemon json to {Path}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving pokemon json {Id}", id);
            }
        }

        public async Task SaveMoveJsonAsync(int id, string folderPath)
        {
            try
            {
                Directory.CreateDirectory(folderPath);

                var moveResponse = await _httpClient.GetStringAsync($"{BaseUrl}/move/{id}");
                var filePath = Path.Combine(folderPath, $"move_{id:D3}.json");
                await File.WriteAllTextAsync(filePath, moveResponse);
                _logger.LogInformation("Saved move json to {Path}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving move json {Id}", id);
            }
        }

        // --- 新規追加: 保存済 JSON からモデルへ変換 ---
        public PokemonSpecies? ParsePokemonSpeciesFromSavedJson(string filePath, int id)
        {
            try
            {
                var txt = File.ReadAllText(filePath);
                using var doc = JsonDocument.Parse(txt);
                var root = doc.RootElement;
                var pokemon = root.GetProperty("pokemon");
                var species = root.GetProperty("species");

                var japaneseName = GetJapaneseName(JsonDocument.Parse(species.GetRawText()), "names");

                var result = new PokemonSpecies
                {
                    PokemonSpeciesId = id,
                    Name = japaneseName,
                    FrontImage = pokemon.GetProperty("sprites").GetProperty("front_default").GetString() ?? "",
                    BackImage = pokemon.GetProperty("sprites").GetProperty("back_default").GetString() ?? "",
                    EvolveLevel = 0,
                    BaseHP = 0,
                    BaseAttack = 0,
                    BaseDefense = 0,
                    BaseSpecialAttack = 0,
                    BaseSpecialDefense = 0,
                    BaseSpeed = 0
                };

                var types = new List<server.Models.Enums.Type>();
                if (pokemon.TryGetProperty("types", out var typesElem))
                {
                    foreach (var t in typesElem.EnumerateArray())
                    {
                        var typeName = t.GetProperty("type").GetProperty("name").GetString() ?? "";
                        types.Add(MapTypeFromString(typeName));
                    }
                }
                result.Type1 = types.Count > 0 ? types[0] : server.Models.Enums.Type.Normal;
                result.Type2 = types.Count > 1 ? types[1] : null;

                if (pokemon.TryGetProperty("stats", out var statsElem))
                {
                    foreach (var statElement in statsElem.EnumerateArray())
                    {
                        var statName = statElement.GetProperty("stat").GetProperty("name").GetString();
                        var baseStat = statElement.GetProperty("base_stat").GetInt32();
                        switch (statName)
                        {
                            case "hp":
                                result.BaseHP = baseStat;
                                break;
                            case "attack":
                                result.BaseAttack = baseStat;
                                break;
                            case "defense":
                                result.BaseDefense = baseStat;
                                break;
                            case "special-attack":
                                result.BaseSpecialAttack = baseStat;
                                break;
                            case "special-defense":
                                result.BaseSpecialDefense = baseStat;
                                break;
                            case "speed":
                                result.BaseSpeed = baseStat;
                                break;
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing pokemon json {Path}", filePath);
                return null;
            }
        }

        public Move? ParseMoveFromSavedJson(string filePath, int id)
        {
            try
            {
                var txt = File.ReadAllText(filePath);
                using var doc = JsonDocument.Parse(txt);
                var root = doc.RootElement;

                var japaneseName = GetJapaneseName(doc, "names");
                var typeName = root.GetProperty("type").GetProperty("name").GetString() ?? "";
                var damageClassName = root.GetProperty("damage_class").GetProperty("name").GetString() ?? "";

                var move = new Move
                {
                    MoveId = id,
                    Name = japaneseName,
                    Type = MapTypeFromString(typeName),
                    Category = MapCategoryFromMeta(doc),
                    DamageClass = MapDamageClassFromString(damageClassName),
                    Power = root.TryGetProperty("power", out var powerProp) && powerProp.ValueKind != JsonValueKind.Null
                        ? powerProp.GetInt32()
                        : 0,
                    Accuracy = root.TryGetProperty("accuracy", out var accuracyProp) && accuracyProp.ValueKind != JsonValueKind.Null
                        ? accuracyProp.GetInt32()
                        : 100,
                    PP = root.TryGetProperty("pp", out var ppProp)
                        ? ppProp.GetInt32()
                        : 0,
                    Priority = root.TryGetProperty("priority", out var priorityProp)
                        ? priorityProp.GetInt32()
                        : 0,
                    Rank = new Rank(),
                    StatTarget = "",
                    StatChance = 0,
                    Ailment = Ailment.None,
                    AilmentChance = 0,
                    Healing = 0,
                    Drain = 0
                };

                if (root.TryGetProperty("meta", out var meta))
                {
                    if (meta.TryGetProperty("ailment", out var ailmentProp))
                    {
                        var ailmentName = ailmentProp.GetProperty("name").GetString() ?? "";
                        move.Ailment = MapAilmentFromString(ailmentName);
                    }
                    if (meta.TryGetProperty("ailment_chance", out var ailmentChanceProp))
                    {
                        move.AilmentChance = ailmentChanceProp.GetInt32();
                    }
                    if (meta.TryGetProperty("drain", out var drainProp))
                    {
                        move.Drain = drainProp.GetInt32();
                    }
                    if (meta.TryGetProperty("healing", out var healingProp))
                    {
                        move.Healing = healingProp.GetInt32();
                    }
                }

                if (root.TryGetProperty("stat_changes", out var statChanges) && statChanges.GetArrayLength() > 0)
                {
                    var firstStatChange = statChanges[0];
                    var statName = firstStatChange.GetProperty("stat").GetProperty("name").GetString() ?? "";
                    var change = firstStatChange.GetProperty("change").GetInt32();

                    move.StatTarget = MapStatName(statName);
                    move.Rank = CreateRankFromStatChange(statName, change);

                    if (root.TryGetProperty("effect_chance", out var effectChance) && effectChance.ValueKind != JsonValueKind.Null)
                    {
                        move.StatChance = effectChance.GetInt32();
                    }
                    else
                    {
                        move.StatChance = 100;
                    }
                }

                return move;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing move json {Path}", filePath);
                return null;
            }
        }

        private string GetJapaneseName(JsonDocument data, string propertyName)
        {
            var names = data.RootElement.GetProperty(propertyName);
            foreach (var name in names.EnumerateArray())
            {
                var language = name.GetProperty("language").GetProperty("name").GetString();
                if (language == "ja" || language == "ja-Hrkt")
                {
                    return name.GetProperty("name").GetString() ?? "";
                }
            }
            return "";
        }

        private server.Models.Enums.Type MapTypeFromString(string typeName)
        {
            return typeName.ToLower() switch
            {
                "normal" => server.Models.Enums.Type.Normal,
                "fire" => server.Models.Enums.Type.Fire,
                "water" => server.Models.Enums.Type.Water,
                "electric" => server.Models.Enums.Type.Electric,
                "grass" => server.Models.Enums.Type.Grass,
                "ice" => server.Models.Enums.Type.Ice,
                "fighting" => server.Models.Enums.Type.Fighting,
                "poison" => server.Models.Enums.Type.Poison,
                "ground" => server.Models.Enums.Type.Ground,
                "flying" => server.Models.Enums.Type.Flying,
                "psychic" => server.Models.Enums.Type.Psychic,
                "bug" => server.Models.Enums.Type.Bug,
                "rock" => server.Models.Enums.Type.Rock,
                "ghost" => server.Models.Enums.Type.Ghost,
                "dragon" => server.Models.Enums.Type.Dragon,
                "dark" => server.Models.Enums.Type.Dark,
                "steel" => server.Models.Enums.Type.Steel,
                "fairy" => server.Models.Enums.Type.Fairy,
                _ => server.Models.Enums.Type.Normal
            };
        }

        private DamageClass MapDamageClassFromString(string damageClassName)
        {
            return damageClassName.ToLower() switch
            {
                "physical" => DamageClass.Physical,
                "special" => DamageClass.Special,
                "status" => DamageClass.Status,
                _ => DamageClass.Status
            };
        }

        private Category MapCategoryFromMeta(JsonDocument moveData)
        {
            if (moveData.RootElement.TryGetProperty("meta", out var meta))
            {
                if (meta.TryGetProperty("category", out var categoryProp))
                {
                    var categoryName = categoryProp.GetProperty("name").GetString() ?? "";
                    return categoryName.ToLower() switch
                    {
                        "damage" => Category.Damage,
                        "ailment" => Category.Ailment,
                        "net-good-stats" => Category.NetGoodStats,
                        "heal" => Category.Heal,
                        "damage+ailment" => Category.DamageAilment,
                        "swagger" => Category.Swagger,
                        "damage+lower" => Category.DamageLower,
                        "damage+raise" => Category.DamageRaise,
                        "damage+heal" => Category.DamageHeal,
                        "ohko" => Category.OHKo,
                        "whole-field-effect" => Category.WholeFieldEffect,
                        "field-effect" => Category.FieldEffect,
                        "force-switch" => Category.ForceSwitch,
                        "unique" => Category.Unique,
                        _ => Category.Damage
                    };
                }
            }
            return Category.Damage;
        }

        private Ailment MapAilmentFromString(string ailmentName)
        {
            return ailmentName.ToLower() switch
            {
                "none" => Ailment.None,
                "paralysis" => Ailment.Paralysis,
                "sleep" => Ailment.Sleep,
                "freeze" => Ailment.Freeze,
                "burn" => Ailment.Burn,
                "poison" => Ailment.Poison,
                "confusion" => Ailment.Confusion,
                "infatuation" => Ailment.Infatuation,
                "trap" => Ailment.Trap,
                "nightmare" => Ailment.Nightmare,
                "torment" => Ailment.Torment,
                "disable" => Ailment.Disable,
                "yawn" => Ailment.Yawn,
                "heal-block" => Ailment.HealBlock,
                "no-type-immunity" => Ailment.NoTypeImmunity,
                "leech-seed" => Ailment.LeechSeed,
                "embargo" => Ailment.Embargo,
                "perish-song" => Ailment.PerishSong,
                "ingrain" => Ailment.Ingrain,
                "silence" => Ailment.Silence,
                "tar-shot" => Ailment.TarShot,
                _ => Ailment.None
            };
        }

        private string MapStatName(string statName)
        {
            return statName.ToLower() switch
            {
                "attack" => "Attack",
                "defense" => "Defense",
                "special-attack" => "SpecialAttack",
                "special-defense" => "SpecialDefense",
                "speed" => "Speed",
                "accuracy" => "Accuracy",
                "evasion" => "Evasion",
                _ => ""
            };
        }

        private Rank CreateRankFromStatChange(string statName, int change)
        {
            var rank = new Rank();
            
            switch (statName.ToLower())
            {
                case "attack":
                    rank.Attack = change;
                    break;
                case "defense":
                    rank.Defense = change;
                    break;
                case "special-attack":
                    rank.SpecialAttack = change;
                    break;
                case "special-defense":
                    rank.SpecialDefense = change;
                    break;
                case "speed":
                    rank.Speed = change;
                    break;
                case "accuracy":
                    rank.Accuracy = change;
                    break;
                case "evasion":
                    rank.Evasion = change;
                    break;
            }
            
            return rank;
        }
    }
}