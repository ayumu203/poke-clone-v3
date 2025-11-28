using Server.Domain.Entities;
using Server.Domain.Enums;

namespace Server.Domain.Services;

public interface IDamageCalculator
{
    int CalcDamage(Pokemon attacker, Pokemon defender, Move move, double typeEffectiveness);
}
