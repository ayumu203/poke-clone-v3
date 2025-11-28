using Server.Domain.Entities;
using Server.Domain.Enums;

namespace Server.Domain.Services;

public class DamageCalculator : IDamageCalculator
{
    private readonly IStatCalculator _statCalculator;

    public DamageCalculator(IStatCalculator statCalculator)
    {
        _statCalculator = statCalculator;
    }

    public int CalcDamage(Pokemon attacker, Pokemon defender, Move move, double typeEffectiveness)
    {
        if (move.Power == 0)
        {
            return 0;
        }

        var level = attacker.Level;
        var power = move.Power;

        int attackStat;
        int defenseStat;

        if (move.DamageClass == DamageClass.Physical)
        {
            attackStat = _statCalculator.CalcStat(attacker.Level, attacker.Species.BaseAttack);
            defenseStat = _statCalculator.CalcStat(defender.Level, defender.Species.BaseDefence);
        }
        else if (move.DamageClass == DamageClass.Special)
        {
            attackStat = _statCalculator.CalcStat(attacker.Level, attacker.Species.BaseSpecialAttack);
            defenseStat = _statCalculator.CalcStat(defender.Level, defender.Species.BaseSpecialDefence);
        }
        else
        {
            return 0;
        }

        var stab = (move.Type == attacker.Species.Type1 || (attacker.Species.Type2.HasValue && move.Type == attacker.Species.Type2.Value)) ? 1.5 : 1.0;

        var baseDamage = ((2 * level / 5 + 2) * power * attackStat / defenseStat) / 50 + 2;

        var damage = (int)(baseDamage * stab * typeEffectiveness);

        return Math.Max(damage, 1);
    }
}
