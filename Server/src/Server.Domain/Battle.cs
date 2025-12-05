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
        var result = new ActionResult
        {
            ActionPokemonId = actor.Party[actor.ActivePokemonIndex].PokemonId,
            ActionType = action.ActionType
        };

        switch (action.ActionType)
        {
            case Enums.ActionType.Attack:
                result.MoveResult = ProcessMoveAction(actor, target, action.Value);
                break;

            case Enums.ActionType.Switch:
                result.SwitchResult = ProcessSwitchAction(actor, action.Value);
                break;

            case Enums.ActionType.Catch:
                result.CatchResult = ProcessCatchAction(target);
                break;
            case Enums.ActionType.Escape:
                result.EscapeResult = ProcessEscapeAction(actor, target);
                break;
        }

        return result;
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
        // 変化技も命中判定は行う
        var random = new Random();
        if (move.Accuracy > 0) // 必中技(Accuracy=0/null)以外
        {
            // 命中ランク補正などを考慮すべきだが、まずは簡易実装
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
        }

        // タイプ相性の計算
        var typeEffectiveness = _typeEffectivenessManager.GetEffectiveness(
            move.Type, defenderPokemon.Species.Type1);

        if (defenderPokemon.Species.Type2.HasValue)
        {
            typeEffectiveness *= _typeEffectivenessManager.GetEffectiveness(
                move.Type, defenderPokemon.Species.Type2.Value);
        }

        // タイプ相性で無効化された場合（例：地面に電気技）
        // 変化技でも無効化される（例：地面に電磁波）
        if (typeEffectiveness == 0)
        {
            return new MoveResult
            {
                MoveId = moveId,
                TargetId = defenderPokemon.PokemonId,
                IsSuccess = false,
                FailureReason = "It doesn't affect..."
            };
        }

        int damage = 0;
        bool isCritical = false;

        // ダメージ計算（物理・特殊技のみ）
        if (move.DamageClass != Enums.DamageClass.Status)
        {
            damage = _damageCalculator.CalcDamage(
                attackerPokemon, defenderPokemon, move, typeEffectiveness);

            // クリティカル判定
            // CritRate: 0=1/16, 1=1/8, 2=1/2, 3+=1/1
            // 簡易実装
            int critChanceDenominator = move.CritRate switch
            {
                0 => 16,
                1 => 8,
                2 => 2,
                _ => 1
            };

            isCritical = random.Next(0, critChanceDenominator) == 0;
            if (isCritical)
            {
                damage = (int)(damage * 1.5);
            }
        }

        // 状態異常の判定
        Enums.Ailment? ailment = null;
        if (move.Ailment != Enums.Ailment.None)
        {
            // AilmentChanceが0の場合は50%とみなす（簡易実装）
            var chance = move.AilmentChance == 0 ? 50 : move.AilmentChance;
            if (random.Next(1, 101) <= chance)
            {
                // 既に状態異常にかかっているかどうかのチェックはBattleService/BattleState側で行う想定
                // ここでは「付与成功」の結果を返す
                ailment = move.Ailment;
            }
        }

        // ステータス変化
        var sourceStatChanges = new List<StatChange>();
        var targetStatChanges = new List<StatChange>();

        foreach (var change in move.StatChanges)
        {
            // StatChance による確率判定
            // StatChance が 0 の場合は 100% として扱う
            var statChance = move.StatChance == 0 ? 100 : move.StatChance;
            if (random.Next(1, 101) > statChance)
            {
                // 確率により発動しなかった
                continue;
            }

            // 対象判定（簡易ロジック）
            // 変化技で上昇 -> 自分
            // それ以外 -> 相手
            bool isSelfTarget = move.DamageClass == Enums.DamageClass.Status && change.Change > 0;

            if (isSelfTarget)
            {
                sourceStatChanges.Add(change);
            }
            else
            {
                // 変化技で下降、または攻撃技の追加効果 -> 相手
                targetStatChanges.Add(change);
            }
        }

        // 回復
        int healing = 0;
        if (move.Healing > 0)
        {
            var maxHp = _statCalculator.CalcHp(attackerPokemon.Level, attackerPokemon.Species.BaseHp);
            healing = (int)(maxHp * (move.Healing / 100.0));
        }

        // ドレイン
        int drain = 0;
        if (move.Drain != 0 && damage > 0)
        {
            drain = (int)(damage * (move.Drain / 100.0));
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
            },
            SourceStatChanges = sourceStatChanges,
            TargetStatChanges = targetStatChanges,
            Ailment = ailment,
            Healing = healing,
            Drain = drain
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
        // 捕獲処理は野生ポケモン戦のみ有効
        var targetPokemon = target.Party[target.ActivePokemonIndex];
        
        // ポケモンの捕獲率計算
        // 実際のポケモンの捕獲率計算式を参考に実装
        // 捕獲率 = ((3 * MaxHP - 2 * CurrentHP) * CatchRate * BonusStatus) / (3 * MaxHP)
        // 簡易実装のため、BattleStateから現在HPを取得できないため、
        // HPに基づく基本捕獲率と状態異常ボーナスのみを考慮
        
        var random = new Random();
        
        // 基本捕獲率（簡易版: 0-100のランダム値）
        // 実際の実装では、BattleStateのCurrentHPを使用すべき
        // ここでは、HPが低いほど捕獲しやすいという概念を簡易的に実装
        // 注: 実際のHP情報はBattleServiceで管理されているため、
        // ここではランダム値を使用
        var baseCatchRate = random.Next(0, 100);
        
        // 状態異常によるボーナス（簡易版）
        // 実際の実装では、BattleStateのAilmentを参照すべき
        // sleep/freeze: 2.5倍, paralysis/poison/burn: 1.5倍, none: 1.0倍
        // ここでは状態異常情報が取得できないため、ボーナスなしで実装
        double statusBonus = 1.0;
        
        // 捕獲成功判定
        // 基本捕獲率が50以上で成功（簡易版）
        var isSuccess = baseCatchRate >= 50;

        return new CatchResult
        {
            IsSuccess = isSuccess,
            CaughtPokemonId = isSuccess ? targetPokemon.PokemonId : string.Empty
        };
    }

    private EscapeResult ProcessEscapeAction(BattlePlayer actor, BattlePlayer opponent)
    {
        // CPUバトル(野生ポケモン戦)の場合は100%成功
        // 対人バトルでは逃走不可
        bool isCpuBattle = opponent.Player.PlayerId == "CPU";
        
        return new EscapeResult
        {
            IsSuccess = isCpuBattle,
            EscapingPlayerId = actor.Player.PlayerId,
            FailureReason = isCpuBattle ? string.Empty : "対人戦では逃走できません"
        };
    }
}
