using Server.Domain.Enums;

namespace Server.Domain.Entities;

public class StatChange
{
    public PokemonStat Stat { get; set; }
    public int Change { get; set; }
}
