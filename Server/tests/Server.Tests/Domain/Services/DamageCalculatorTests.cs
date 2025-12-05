using Server.Domain.Entities;
using Server.Domain.Enums;
using Server.Domain.Services;
using Xunit;

namespace Server.Tests.Domain.Services;

public class DamageCalculatorTests
{
    private readonly DamageCalculator _calculator;
    private readonly StatCalculator _statCalculator;

    public DamageCalculatorTests()
    {
        _statCalculator = new StatCalculator();
        _calculator = new DamageCalculator(_statCalculator);
    }

    [Fact]
    public void CalcDamage_PhysicalMoveWithoutSTAB_ReturnsCorrectDamage()
    {
        var attacker = new Pokemon
        {
            Level = 50,
            Species = new PokemonSpecies
            {
                BaseAttack = 100,
                Type1 = PokemonType.Normal
            }
        };

        var defender = new Pokemon
        {
            Level = 50,
            Species = new PokemonSpecies
            {
                BaseDefence = 100
            }
        };

        var move = new Move
        {
            Power = 80,
            DamageClass = DamageClass.Physical,
            Type = PokemonType.Fighting
        };

        var damage = _calculator.CalcDamage(attacker, defender, move, 1.0);

        Assert.True(damage > 0);
    }

    [Fact]
    public void CalcDamage_StatusMove_Returns0()
    {
        var attacker = new Pokemon
        {
            Level = 50,
            Species = new PokemonSpecies { BaseAttack = 100 }
        };

        var defender = new Pokemon
        {
            Level = 50,
            Species = new PokemonSpecies { BaseDefence = 100 }
        };

        var move = new Move
        {
            Power = 0,
            DamageClass = DamageClass.Status
        };

        var damage = _calculator.CalcDamage(attacker, defender, move, 1.0);

        Assert.Equal(0, damage);
    }
}
