using Server.Domain.Enums;

namespace Server.Domain.Entities;

public class Move
{
    public int MoveId { get; set; }
    public string Name { get; set; } = string.Empty;
    public PokemonType Type { get; set; }
    public Category Category { get; set; }
    public DamageClass DamageClass { get; set; }
    public int Power { get; set; }
    public int Accuracy { get; set; }
    public int Pp { get; set; }
    public int Priority { get; set; }
    public Rank Rank { get; set; } = new();
    public string RankTarget { get; set; } = string.Empty;
    public int RankChance { get; set; }
    public Ailment Ailment { get; set; }
    public int AilmentChance { get; set; }
    public int Healing { get; set; }
    public int Drain { get; set; }
}
