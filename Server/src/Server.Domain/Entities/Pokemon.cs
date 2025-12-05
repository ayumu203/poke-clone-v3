namespace Server.Domain.Entities;

public class Pokemon
{
    public string PokemonId { get; set; } = string.Empty;
    public int PokemonSpeciesId { get; set; }
    public PokemonSpecies? Species { get; set; }
    public int Level { get; set; }
    public int Exp { get; set; }
    public List<Move> Moves { get; set; } = new();
}
