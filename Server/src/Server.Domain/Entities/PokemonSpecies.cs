using Server.Domain.Enums;

namespace Server.Domain.Entities;

public class PokemonSpecies
{
    public int PokemonSpeciesId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FrontImage { get; set; } = string.Empty;
    public string BackImage { get; set; } = string.Empty;
    public PokemonType Type1 { get; set; }
    public PokemonType? Type2 { get; set; }
    public int EvolveLevel { get; set; }
    public int BaseHp { get; set; }
    public int BaseAttack { get; set; }
    public int BaseDefence { get; set; }
    public int BaseSpecialAttack { get; set; }
    public int BaseSpecialDefence { get; set; }
    public int BaseSpeed { get; set; }
    public List<Move> MoveList { get; set; } = new();
}
