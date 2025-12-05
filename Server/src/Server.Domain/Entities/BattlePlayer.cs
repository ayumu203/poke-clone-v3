namespace Server.Domain.Entities;

public class BattlePlayer
{
    public Player Player { get; set; } = new();
    public List<Pokemon> Party { get; set; } = new();
    public int ActivePokemonIndex { get; set; }
}
