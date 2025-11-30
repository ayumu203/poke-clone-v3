using Server.Domain;
using Server.Domain.Entities;
using Server.Domain.Enums;
using Server.Domain.Services;
using Xunit;

namespace Server.Tests.Domain;

public class BattleTests
{
    // Mock implementations
    class MockDamageCalculator : IDamageCalculator
    {
        public int CalcDamage(Pokemon attacker, Pokemon defender, Move move, double typeEffectiveness) => 10;
    }

    class MockTypeEffectivenessManager : ITypeEffectivenessManager
    {
        public double GetEffectiveness(PokemonType attackType, PokemonType defenseType) => 1.0;
    }

    class MockStatCalculator : IStatCalculator
    {
        public int CalcHp(int level, int baseStat) => 100;
        public int CalcStat(int level, int baseStat) => 50;
        public int CalcSpeed(int level, int baseStat) => 50;
    }

    [Fact]
    public void ProcessTurn_StatusMove_ShouldApplyStatChanges()
    {
        // Arrange
        var damageCalc = new MockDamageCalculator();
        var typeEff = new MockTypeEffectivenessManager();
        var statCalc = new MockStatCalculator();

        var p1 = new BattlePlayer { Player = new Player { PlayerId = "p1" } };
        var p2 = new BattlePlayer { Player = new Player { PlayerId = "p2" } };

        var poke1 = new Pokemon 
        { 
            PokemonId = "poke1", 
            Species = new PokemonSpecies { BaseSpeed = 50 },
            Moves = new List<Move> 
            { 
                new Move 
                { 
                    MoveId = 1, 
                    Name = "Swords Dance", 
                    DamageClass = DamageClass.Status,
                    StatChanges = new List<StatChange> 
                    { 
                        new StatChange { Stat = PokemonStat.Attack, Change = 2 } 
                    },
                    Accuracy = 100
                } 
            } 
        };
        p1.Party.Add(poke1);
        p1.ActivePokemonIndex = 0;

        var poke2 = new Pokemon 
        { 
            PokemonId = "poke2", 
            Species = new PokemonSpecies { BaseSpeed = 50 } 
        };
        p2.Party.Add(poke2);
        p2.ActivePokemonIndex = 0;

        var battle = new Battle(damageCalc, typeEff, statCalc, p1, p2);

        // Act
        var result = battle.ProcessTurn(
            new PlayerAction { ActionType = ActionType.Attack, Value = 1 }, // P1 uses Swords Dance
            new PlayerAction { ActionType = ActionType.Attack, Value = 0 }  // P2 does nothing (dummy)
        );

        // Assert
        // P1 goes first (speed tie -> p1)
        var actionResult = result.ActionResults[0];
        
        Assert.True(actionResult.MoveResult.IsSuccess);
        Assert.Equal(2, actionResult.MoveResult.SourceStatChanges[0].Change);
        Assert.Equal(PokemonStat.Attack, actionResult.MoveResult.SourceStatChanges[0].Stat);
        Assert.Empty(actionResult.MoveResult.TargetStatChanges);
    }

    [Fact]
    public void ProcessTurn_StatusMove_ShouldApplyAilment()
    {
        // Arrange
        var damageCalc = new MockDamageCalculator();
        var typeEff = new MockTypeEffectivenessManager();
        var statCalc = new MockStatCalculator();

        var p1 = new BattlePlayer { Player = new Player { PlayerId = "p1" } };
        var p2 = new BattlePlayer { Player = new Player { PlayerId = "p2" } };

        var poke1 = new Pokemon 
        { 
            PokemonId = "poke1", 
            Species = new PokemonSpecies { BaseSpeed = 60 }, // Faster
            Moves = new List<Move> 
            { 
                new Move 
                { 
                    MoveId = 1, 
                    Name = "Thunder Wave", 
                    DamageClass = DamageClass.Status,
                    Ailment = Ailment.Paralysis,
                    AilmentChance = 100,
                    Accuracy = 100
                } 
            } 
        };
        p1.Party.Add(poke1);
        p1.ActivePokemonIndex = 0;

        var poke2 = new Pokemon 
        { 
            PokemonId = "poke2", 
            Species = new PokemonSpecies { BaseSpeed = 50 } 
        };
        p2.Party.Add(poke2);
        p2.ActivePokemonIndex = 0;

        var battle = new Battle(damageCalc, typeEff, statCalc, p1, p2);

        // Act
        var result = battle.ProcessTurn(
            new PlayerAction { ActionType = ActionType.Attack, Value = 1 },
            new PlayerAction { ActionType = ActionType.Attack, Value = 0 }
        );

        // Assert
        var actionResult = result.ActionResults[0];
        Assert.True(actionResult.MoveResult.IsSuccess);
        Assert.Equal(Ailment.Paralysis, actionResult.MoveResult.Ailment);
    }

    [Fact]
    public void ProcessTurn_HealingMove_ShouldHeal()
    {
        // Arrange
        var damageCalc = new MockDamageCalculator();
        var typeEff = new MockTypeEffectivenessManager();
        var statCalc = new MockStatCalculator();

        var p1 = new BattlePlayer { Player = new Player { PlayerId = "p1" } };
        var p2 = new BattlePlayer { Player = new Player { PlayerId = "p2" } };

        var poke1 = new Pokemon 
        { 
            PokemonId = "poke1", 
            Species = new PokemonSpecies { BaseSpeed = 60, BaseHp = 100 },
            Moves = new List<Move> 
            { 
                new Move 
                { 
                    MoveId = 1, 
                    Name = "Recover", 
                    DamageClass = DamageClass.Status,
                    Healing = 50,
                    Accuracy = 100
                } 
            } 
        };
        p1.Party.Add(poke1);
        p1.ActivePokemonIndex = 0;

        var poke2 = new Pokemon 
        { 
            PokemonId = "poke2", 
            Species = new PokemonSpecies { BaseSpeed = 50 } 
        };
        p2.Party.Add(poke2);
        p2.ActivePokemonIndex = 0;

        var battle = new Battle(damageCalc, typeEff, statCalc, p1, p2);

        // Act
        var result = battle.ProcessTurn(
            new PlayerAction { ActionType = ActionType.Attack, Value = 1 },
            new PlayerAction { ActionType = ActionType.Attack, Value = 0 }
        );

        // Assert
        var actionResult = result.ActionResults[0];
        Assert.True(actionResult.MoveResult.IsSuccess);
        // MockStatCalculator.CalcHp returns 100. Healing is 50%. So 50 healing.
        Assert.Equal(50, actionResult.MoveResult.Healing);
    }
}
