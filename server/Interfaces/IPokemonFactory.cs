using server.Models.Basics;

namespace server.Interfaces
{
    public interface IPokemonFactory
    {
        Task<List<Pokemon>> MakePokemonListAsync(Player player);
    }
}