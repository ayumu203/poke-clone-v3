using Server.Domain.Enums;

namespace Server.Domain.Services;

public interface ITypeEffectivenessManager
{
    double GetEffectiveness(PokemonType attackType, PokemonType defenseType);
}
