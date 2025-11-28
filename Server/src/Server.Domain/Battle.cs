using Server.Domain.Entities;
using Server.Domain.Services;

namespace Server.Domain;

public class Battle
{
    private readonly IDamageCalculator _damageCalculator;
    private readonly ITypeEffectivenessManager _typeEffectivenessManager;
    private readonly IStatCalculator _statCalculator;

    public BattlePlayer Player1 { get; set; }
    public BattlePlayer Player2 { get; set; }

    public Battle(
        IDamageCalculator damageCalculator,
        ITypeEffectivenessManager typeEffectivenessManager,
        IStatCalculator statCalculator,
        BattlePlayer player1,
        BattlePlayer player2)
    {
        _damageCalculator = damageCalculator;
        _typeEffectivenessManager = typeEffectivenessManager;
        _statCalculator = statCalculator;
        Player1 = player1;
        Player2 = player2;
    }

    public ProcessResult ProcessTurn(PlayerAction action1, PlayerAction action2)
    {
        var result = new ProcessResult();

        var pokemon1 = Player1.Party[Player1.ActivePokemonIndex];
        var pokemon2 = Player2.Party[Player2.ActivePokemonIndex];

        var speed1 = _statCalculator.CalcSpeed(pokemon1.Level, pokemon1.Species.BaseSpeed);
        var speed2 = _statCalculator.CalcSpeed(pokemon2.Level, pokemon2.Species.BaseSpeed);

        var firstPlayer = speed1 >= speed2 ? Player1 : Player2;
        var firstAction = speed1 >= speed2 ? action1 : action2;
        var secondPlayer = speed1 >= speed2 ? Player2 : Player1;
        var secondAction = speed1 >= speed2 ? action2 : action1;

        result.ActionResults.Add(ProcessAction(firstPlayer, secondPlayer, firstAction));
        result.ActionResults.Add(ProcessAction(secondPlayer, firstPlayer, secondAction));

        return result;
    }

    private ActionResult ProcessAction(BattlePlayer actor, BattlePlayer target, PlayerAction action)
    {
        var actionResult = new ActionResult
        {
            ActionPokemonId = actor.Party[actor.ActivePokemonIndex].PokemonId,
            ActionType = action.ActionType
        };

        return actionResult;
    }
}
