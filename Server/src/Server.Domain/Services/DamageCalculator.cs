using Server.Domain.Entities;
using Server.Domain.Enums;

namespace Server.Domain.Services;

public class DamageCalculator : IDamageCalculator
{
    private readonly IStatCalculator _statCalculator;
    
    private const double StabMultiplier = 1.5;
    private const double NoStabMultiplier = 1.0;
    private const int DamageFormulaConstant1 = 2;
    private const int DamageFormulaConstant2 = 5;
    private const int DamageFormulaConstant3 = 50;
    private const int MinDamage = 1;

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

        var stab = (move.Type == attacker.Species.Type1 || (attacker.Species.Type2.HasValue && move.Type == attacker.Species.Type2.Value)) ? StabMultiplier : NoStabMultiplier;

        var baseDamage = ((DamageFormulaConstant1 * level / DamageFormulaConstant2 + DamageFormulaConstant1) * power * attackStat / defenseStat) / DamageFormulaConstant3 + DamageFormulaConstant1;

        var damage = (int)(baseDamage * stab * typeEffectiveness);

        return Math.Max(damage, MinDamage);
    }
}
