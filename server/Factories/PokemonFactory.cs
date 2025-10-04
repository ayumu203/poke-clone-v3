using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models.Basics;
using server.Interfaces;

namespace server.Factories
{
    public class PokemonFactory : IPokemonFactory
    {
        private readonly ApplicationDbContext _context;

        public PokemonFactory(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Pokemon>> MakePokemonListAsync(Player player)
        {
            var pokemons = await _context.Pokemons
                .Where(p => p.PlayerId == player.PlayerId)
                .Include(p => p.PokemonSpecies)
                .Include(p => p.Move1)
                .Include(p => p.Move2)
                .Include(p => p.Move3)
                .Include(p => p.Move4)
                .ToListAsync();

            foreach (var pokemon in pokemons)
            {
                pokemon.CurrentHp = CalculateMaxHp(pokemon);
            }

            return pokemons;
        }

        private int CalculateMaxHp(Pokemon pokemon)
        {
            if (pokemon.PokemonSpecies == null)
            {
                return 1;
            }

            // ポケモンのHPを計算する (個体値・努力値は31として計算)
            int maxHp = (int)Math.Floor((double)(pokemon.PokemonSpecies.BaseHp * 2 + 31) * pokemon.Level / 100) + pokemon.Level + 10;
            return maxHp;
        }
    }
}