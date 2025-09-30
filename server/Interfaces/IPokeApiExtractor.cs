using System.Threading.Tasks;

namespace server.Interfaces
{
    public interface IPokeApiExtractor
    {
        Task ExtractPokemonSpeciesAsync(int startId, int endId);
        Task ExtractMovesAsync(int startId, int endId);
    }
}
