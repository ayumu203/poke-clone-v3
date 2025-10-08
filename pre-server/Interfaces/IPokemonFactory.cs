using server.Models.Core;

namespace server.Interfaces
{
    public interface IPokemonFactory
    {
        Task<List<Pokemon>> MakePokemonListAsync(Player player);
    }
}