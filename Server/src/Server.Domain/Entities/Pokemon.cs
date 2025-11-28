namespace Server.Domain.Entities;

public class Pokemon
{
    public string PokemonId { get; set; } = string.Empty;
    public PokemonSpecies Species { get; set; } = new();
    public int Level { get; set; }
    public int Exp { get; set; }
    public List<Move> Moves { get; set; } = new();
}
