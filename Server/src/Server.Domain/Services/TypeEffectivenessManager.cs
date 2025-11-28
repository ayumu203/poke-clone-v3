using Server.Domain.Enums;

namespace Server.Domain.Services;

public class TypeEffectivenessManager : ITypeEffectivenessManager
{
    private readonly Dictionary<(PokemonType, PokemonType), double> _effectivenessTable;

    public TypeEffectivenessManager()
    {
        _effectivenessTable = InitializeEffectivenessTable();
    }

    public double GetEffectiveness(PokemonType attackType, PokemonType defenseType)
    {
        return _effectivenessTable.TryGetValue((attackType, defenseType), out var effectiveness)
            ? effectiveness
            : 1.0;
    }

    private static Dictionary<(PokemonType, PokemonType), double> InitializeEffectivenessTable()
    {
        var table = new Dictionary<(PokemonType, PokemonType), double>();

        table[(PokemonType.Fire, PokemonType.Grass)] = 2.0;
        table[(PokemonType.Fire, PokemonType.Ice)] = 2.0;
        table[(PokemonType.Fire, PokemonType.Bug)] = 2.0;
        table[(PokemonType.Fire, PokemonType.Steel)] = 2.0;
        table[(PokemonType.Fire, PokemonType.Fire)] = 0.5;
        table[(PokemonType.Fire, PokemonType.Water)] = 0.5;
        table[(PokemonType.Fire, PokemonType.Rock)] = 0.5;
        table[(PokemonType.Fire, PokemonType.Dragon)] = 0.5;

        table[(PokemonType.Water, PokemonType.Fire)] = 2.0;
        table[(PokemonType.Water, PokemonType.Ground)] = 2.0;
        table[(PokemonType.Water, PokemonType.Rock)] = 2.0;
        table[(PokemonType.Water, PokemonType.Water)] = 0.5;
        table[(PokemonType.Water, PokemonType.Grass)] = 0.5;
        table[(PokemonType.Water, PokemonType.Dragon)] = 0.5;

        table[(PokemonType.Grass, PokemonType.Water)] = 2.0;
        table[(PokemonType.Grass, PokemonType.Ground)] = 2.0;
        table[(PokemonType.Grass, PokemonType.Rock)] = 2.0;
        table[(PokemonType.Grass, PokemonType.Fire)] = 0.5;
        table[(PokemonType.Grass, PokemonType.Grass)] = 0.5;
        table[(PokemonType.Grass, PokemonType.Poison)] = 0.5;
        table[(PokemonType.Grass, PokemonType.Flying)] = 0.5;
        table[(PokemonType.Grass, PokemonType.Bug)] = 0.5;
        table[(PokemonType.Grass, PokemonType.Dragon)] = 0.5;
        table[(PokemonType.Grass, PokemonType.Steel)] = 0.5;

        table[(PokemonType.Electric, PokemonType.Water)] = 2.0;
        table[(PokemonType.Electric, PokemonType.Flying)] = 2.0;
        table[(PokemonType.Electric, PokemonType.Electric)] = 0.5;
        table[(PokemonType.Electric, PokemonType.Grass)] = 0.5;
        table[(PokemonType.Electric, PokemonType.Dragon)] = 0.5;
        table[(PokemonType.Electric, PokemonType.Ground)] = 0.0;

        table[(PokemonType.Ice, PokemonType.Grass)] = 2.0;
        table[(PokemonType.Ice, PokemonType.Ground)] = 2.0;
        table[(PokemonType.Ice, PokemonType.Flying)] = 2.0;
        table[(PokemonType.Ice, PokemonType.Dragon)] = 2.0;
        table[(PokemonType.Ice, PokemonType.Fire)] = 0.5;
        table[(PokemonType.Ice, PokemonType.Water)] = 0.5;
        table[(PokemonType.Ice, PokemonType.Ice)] = 0.5;
        table[(PokemonType.Ice, PokemonType.Steel)] = 0.5;

        table[(PokemonType.Fighting, PokemonType.Normal)] = 2.0;
        table[(PokemonType.Fighting, PokemonType.Ice)] = 2.0;
        table[(PokemonType.Fighting, PokemonType.Rock)] = 2.0;
        table[(PokemonType.Fighting, PokemonType.Dark)] = 2.0;
        table[(PokemonType.Fighting, PokemonType.Steel)] = 2.0;
        table[(PokemonType.Fighting, PokemonType.Poison)] = 0.5;
        table[(PokemonType.Fighting, PokemonType.Flying)] = 0.5;
        table[(PokemonType.Fighting, PokemonType.Psychic)] = 0.5;
        table[(PokemonType.Fighting, PokemonType.Bug)] = 0.5;
        table[(PokemonType.Fighting, PokemonType.Fairy)] = 0.5;
        table[(PokemonType.Fighting, PokemonType.Ghost)] = 0.0;

        table[(PokemonType.Poison, PokemonType.Grass)] = 2.0;
        table[(PokemonType.Poison, PokemonType.Fairy)] = 2.0;
        table[(PokemonType.Poison, PokemonType.Poison)] = 0.5;
        table[(PokemonType.Poison, PokemonType.Ground)] = 0.5;
        table[(PokemonType.Poison, PokemonType.Rock)] = 0.5;
        table[(PokemonType.Poison, PokemonType.Ghost)] = 0.5;
        table[(PokemonType.Poison, PokemonType.Steel)] = 0.0;

        table[(PokemonType.Ground, PokemonType.Fire)] = 2.0;
        table[(PokemonType.Ground, PokemonType.Electric)] = 2.0;
        table[(PokemonType.Ground, PokemonType.Poison)] = 2.0;
        table[(PokemonType.Ground, PokemonType.Rock)] = 2.0;
        table[(PokemonType.Ground, PokemonType.Steel)] = 2.0;
        table[(PokemonType.Ground, PokemonType.Grass)] = 0.5;
        table[(PokemonType.Ground, PokemonType.Bug)] = 0.5;
        table[(PokemonType.Ground, PokemonType.Flying)] = 0.0;

        table[(PokemonType.Flying, PokemonType.Grass)] = 2.0;
        table[(PokemonType.Flying, PokemonType.Fighting)] = 2.0;
        table[(PokemonType.Flying, PokemonType.Bug)] = 2.0;
        table[(PokemonType.Flying, PokemonType.Electric)] = 0.5;
        table[(PokemonType.Flying, PokemonType.Rock)] = 0.5;
        table[(PokemonType.Flying, PokemonType.Steel)] = 0.5;

        table[(PokemonType.Psychic, PokemonType.Fighting)] = 2.0;
        table[(PokemonType.Psychic, PokemonType.Poison)] = 2.0;
        table[(PokemonType.Psychic, PokemonType.Psychic)] = 0.5;
        table[(PokemonType.Psychic, PokemonType.Steel)] = 0.5;
        table[(PokemonType.Psychic, PokemonType.Dark)] = 0.0;

        table[(PokemonType.Bug, PokemonType.Grass)] = 2.0;
        table[(PokemonType.Bug, PokemonType.Psychic)] = 2.0;
        table[(PokemonType.Bug, PokemonType.Dark)] = 2.0;
        table[(PokemonType.Bug, PokemonType.Fire)] = 0.5;
        table[(PokemonType.Bug, PokemonType.Fighting)] = 0.5;
        table[(PokemonType.Bug, PokemonType.Poison)] = 0.5;
        table[(PokemonType.Bug, PokemonType.Flying)] = 0.5;
        table[(PokemonType.Bug, PokemonType.Ghost)] = 0.5;
        table[(PokemonType.Bug, PokemonType.Steel)] = 0.5;
        table[(PokemonType.Bug, PokemonType.Fairy)] = 0.5;

        table[(PokemonType.Rock, PokemonType.Fire)] = 2.0;
        table[(PokemonType.Rock, PokemonType.Ice)] = 2.0;
        table[(PokemonType.Rock, PokemonType.Flying)] = 2.0;
        table[(PokemonType.Rock, PokemonType.Bug)] = 2.0;
        table[(PokemonType.Rock, PokemonType.Fighting)] = 0.5;
        table[(PokemonType.Rock, PokemonType.Ground)] = 0.5;
        table[(PokemonType.Rock, PokemonType.Steel)] = 0.5;

        table[(PokemonType.Ghost, PokemonType.Psychic)] = 2.0;
        table[(PokemonType.Ghost, PokemonType.Ghost)] = 2.0;
        table[(PokemonType.Ghost, PokemonType.Dark)] = 0.5;
        table[(PokemonType.Ghost, PokemonType.Normal)] = 0.0;

        table[(PokemonType.Dragon, PokemonType.Dragon)] = 2.0;
        table[(PokemonType.Dragon, PokemonType.Steel)] = 0.5;
        table[(PokemonType.Dragon, PokemonType.Fairy)] = 0.0;

        table[(PokemonType.Dark, PokemonType.Psychic)] = 2.0;
        table[(PokemonType.Dark, PokemonType.Ghost)] = 2.0;
        table[(PokemonType.Dark, PokemonType.Fighting)] = 0.5;
        table[(PokemonType.Dark, PokemonType.Dark)] = 0.5;
        table[(PokemonType.Dark, PokemonType.Fairy)] = 0.5;

        table[(PokemonType.Steel, PokemonType.Ice)] = 2.0;
        table[(PokemonType.Steel, PokemonType.Rock)] = 2.0;
        table[(PokemonType.Steel, PokemonType.Fairy)] = 2.0;
        table[(PokemonType.Steel, PokemonType.Fire)] = 0.5;
        table[(PokemonType.Steel, PokemonType.Water)] = 0.5;
        table[(PokemonType.Steel, PokemonType.Electric)] = 0.5;
        table[(PokemonType.Steel, PokemonType.Steel)] = 0.5;

        table[(PokemonType.Fairy, PokemonType.Fighting)] = 2.0;
        table[(PokemonType.Fairy, PokemonType.Dragon)] = 2.0;
        table[(PokemonType.Fairy, PokemonType.Dark)] = 2.0;
        table[(PokemonType.Fairy, PokemonType.Fire)] = 0.5;
        table[(PokemonType.Fairy, PokemonType.Poison)] = 0.5;
        table[(PokemonType.Fairy, PokemonType.Steel)] = 0.5;

        return table;
    }
}
