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

        // 先攻のアクション処理
        var firstResult = ProcessAction(firstPlayer, secondPlayer, firstAction);
        result.ActionResults.Add(firstResult);

        // 後攻のポケモンがひんしになっていないか確認
        // 注: 実際のHP管理はBattleStateで行うため、ここではダメージ情報のみ返す
        
        // 後攻のアクション処理
        var secondResult = ProcessAction(secondPlayer, firstPlayer, secondAction);
        result.ActionResults.Add(secondResult);

        // 勝敗判定はBattleServiceで行う（BattleStateのHP情報を基に）
        // ここでは処理結果のみを返す

        return result;
    }

    private ActionResult ProcessAction(BattlePlayer actor, BattlePlayer target, PlayerAction action)
    {
        var actionResult = new ActionResult
        {
            ActionPokemonId = actor.Party[actor.ActivePokemonIndex].PokemonId,
            ActionType = action.ActionType
        };

        switch (action.ActionType)
        {
            case Enums.ActionType.Attack:
                actionResult.MoveResult = ProcessMoveAction(actor, target, action.Value);
                break;

            case Enums.ActionType.Switch:
                actionResult.SwitchResult = ProcessSwitchAction(actor, action.Value);
                break;

            case Enums.ActionType.Catch:
                actionResult.CatchResult = ProcessCatchAction(target);
                break;
        }

        return actionResult;
    }

    //TODO: 状態異常・ステータス上昇技の処理の追加
    private MoveResult ProcessMoveAction(BattlePlayer actor, BattlePlayer target, int moveId)
    {
        var attackerPokemon = actor.Party[actor.ActivePokemonIndex];
        var defenderPokemon = target.Party[target.ActivePokemonIndex];

        var move = attackerPokemon.Moves.FirstOrDefault(m => m.MoveId == moveId);
        if (move == null)
        {
            return new MoveResult
            {
                MoveId = moveId,
                TargetId = defenderPokemon.PokemonId,
                IsSuccess = false,
                FailureReason = "Move not found"
            };
        }

        // 命中判定（Accuracyに基づく判定）
        var random = new Random();
        var hitRoll = random.Next(1, 101); // 1-100の乱数
        if (hitRoll > move.Accuracy)
        {
            return new MoveResult
            {
                MoveId = moveId,
                TargetId = defenderPokemon.PokemonId,
                IsSuccess = false,
                FailureReason = "Move missed"
            };
        }

        // タイプ相性の計算
        var typeEffectiveness = _typeEffectivenessManager.GetEffectiveness(
            move.Type, defenderPokemon.Species.Type1);

        if (defenderPokemon.Species.Type2.HasValue)
        {
            typeEffectiveness *= _typeEffectivenessManager.GetEffectiveness(
                move.Type, defenderPokemon.Species.Type2.Value);
        }

        // ダメージ計算
        var damage = _damageCalculator.CalcDamage(
            attackerPokemon, defenderPokemon, move, typeEffectiveness);

        // クリティカル判定（簡易版：6.25%の確率）
        var isCritical = random.Next(0, 16) == 0;
        if (isCritical)
        {
            damage = (int)(damage * 1.5);
        }

        return new MoveResult
        {
            MoveId = moveId,
            TargetId = defenderPokemon.PokemonId,
            IsSuccess = true,
            Damage = damage,
            HitContext = new HitContext
            {
                IsCritical = isCritical,
                TypeEffectiveness = typeEffectiveness
            }
        };
    }

    private SwitchResult ProcessSwitchAction(BattlePlayer actor, int targetIndex)
    {
        if (targetIndex < 0 || targetIndex >= actor.Party.Count)
        {
            throw new ArgumentException("Invalid switch target index");
        }

        var currentPokemon = actor.Party[actor.ActivePokemonIndex];
        var nextPokemon = actor.Party[targetIndex];

        actor.ActivePokemonIndex = targetIndex;

        return new SwitchResult
        {
            CurrentPokemonId = currentPokemon.PokemonId,
            NextPokemonId = nextPokemon.PokemonId
        };
    }

    private CatchResult ProcessCatchAction(BattlePlayer target)
    {
        // 捕獲処理は野生ポケモン戦のみ有効（簡易実装）
        // ここでは基本的な捕獲ロジックのみ実装
        var targetPokemon = target.Party[target.ActivePokemonIndex];
        
        // 簡易的な捕獲判定（野生ポケモンのHPに基づく）
        // 実際の実装では、ボールの種類、状態異常、HP残量などを考慮する
        var random = new Random();
        var catchRate = random.Next(0, 100);
        var isSuccess = catchRate < 50; // 50%の確率で捕獲成功（簡易版）

        return new CatchResult
        {
            IsSuccess = isSuccess,
            CaughtPokemonId = isSuccess ? targetPokemon.PokemonId : string.Empty
        };
    }
}
