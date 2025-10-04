using server.Models.Basics;
using System.Text.Json;

namespace server.Models.Battles
{
    public class CpuPlayer
    {
        public string PlayerId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "CPU";
        public string IconUrl { get; set; } = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-v/black-white/animated/1.gif";
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public CpuPlayer(HttpClient httpClient, string baseApiUrl = "https://localhost:5259")
        {
            _httpClient = httpClient;
            _baseApiUrl = baseApiUrl;
        }
        public async Task<List<Pokemon>> MakeRandomPokemonList()
        {
            var partyPokemons = new List<Pokemon>();
            var random = new Random();
            const int partySize = 6;
            const int pokemonMaxId = 151;

            for (int i = 0; i < partySize; i++)
            {
                var randomPokemonSpeciesId = random.Next(1, pokemonMaxId + 1);

                PokemonSpecies? species;
                try
                {
                    var response = await _httpClient.GetAsync($"{_baseApiUrl}/api/pokemonspecies/{randomPokemonSpeciesId}");
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    species = JsonSerializer.Deserialize<PokemonSpecies>(content, options);

                    if (species == null)
                    {
                        continue;
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Error fetching PokemonSpecies with ID {randomPokemonSpeciesId}: {ex.Message}");
                    continue;
                }

                var availableMoves = species.Moves.ToList();
                var selectedMoves = availableMoves.OrderBy(x => Guid.NewGuid()).Take(4).ToList();
                var randomLevel = random.Next(1, 101);
                // HPの実数値計算(なお個体値は31, 努力値は0, 性格補正なし)
                var actualHp = ((species.BaseHp * 2 + 31) * randomLevel / 100) + randomLevel + 10;

                var pokemon = new Pokemon
                {
                    PlayerId = this.PlayerId,
                    PokemonSpeciesId = species.PokemonSpeciesId,
                    PokemonSpecies = species,
                    Level = randomLevel,
                    Exp = 0,
                    CurrentHp = actualHp,
                    Move1Id = selectedMoves.Count > 0 ? selectedMoves[0].MoveId : null,
                    Move2Id = selectedMoves.Count > 1 ? selectedMoves[1].MoveId : null,
                    Move3Id = selectedMoves.Count > 2 ? selectedMoves[2].MoveId : null,
                    Move4Id = selectedMoves.Count > 3 ? selectedMoves[3].MoveId : null,
                    Move1 = selectedMoves.Count > 0 ? selectedMoves[0] : null,
                    Move2 = selectedMoves.Count > 1 ? selectedMoves[1] : null,
                    Move3 = selectedMoves.Count > 2 ? selectedMoves[2] : null,
                    Move4 = selectedMoves.Count > 3 ? selectedMoves[3] : null,
                };

                partyPokemons.Add(pokemon);
            }
            return partyPokemons;
        }
        public PlayerAction ChooseAction(Battle battle)
        {
            return new PlayerAction
            {
                ActionType = ActionType.Move,
                Value = 1,
            };
        }
    }
}
