using Server.Domain.Enums;

namespace Server.Domain.Entities;

public class HitContext
{
    public bool IsCritical { get; set; }
    public double TypeEffectiveness { get; set; }
}

public class MoveResult
{
    public int MoveId { get; set; }
    public string TargetId { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public string FailureReason { get; set; } = string.Empty;
    public int Damage { get; set; }
    public HitContext? HitContext { get; set; }
    public List<StatChange> SourceStatChanges { get; set; } = new();
    public List<StatChange> TargetStatChanges { get; set; } = new();
    public Ailment? Ailment { get; set; }
    public int Healing { get; set; }
    public int Drain { get; set; }
}

public class SwitchResult
{
    public string CurrentPokemonId { get; set; } = string.Empty;
    public string NextPokemonId { get; set; } = string.Empty;
}

public class CatchResult
{
    public bool IsSuccess { get; set; }
    public string CaughtPokemonId { get; set; } = string.Empty;
}

public class ActionResult
{
    public string ActionPokemonId { get; set; } = string.Empty;
    public ActionType ActionType { get; set; }
    public MoveResult? MoveResult { get; set; }
    public SwitchResult? SwitchResult { get; set; }
    public CatchResult? CatchResult { get; set; }
    public EscapeResult? EscapeResult { get; set; }
}

/// <summary>
/// 状態異常によるダメージ情報
/// </summary>
public class AilmentDamage
{
    public string PokemonId { get; set; } = string.Empty;
    public Ailment Ailment { get; set; }
    public int Damage { get; set; }
    public int HpBefore { get; set; }
    public int HpAfter { get; set; }
}

/// <summary>
/// HP変化情報（ターン結果としてクライアントに送信）
/// </summary>
public class HpChange
{
    public string PokemonId { get; set; } = string.Empty;
    public int HpBefore { get; set; }
    public int HpAfter { get; set; }
    public int MaxHp { get; set; }
}

/// <summary>
/// 強制交代情報（ポケモンが瀕死になった時）
/// </summary>
public class ForcedSwitch
{
    public string PlayerId { get; set; } = string.Empty;
    public string FaintedPokemonId { get; set; } = string.Empty;
    public string NextPokemonId { get; set; } = string.Empty;
}

public class ProcessResult
{
    public List<ActionResult> ActionResults { get; set; } = new();
    public bool IsBattleEnd { get; set; }
    public string WinnerId { get; set; } = string.Empty;
    public EndResult? EndResult { get; set; }
    
    /// <summary>
    /// 状態異常によるダメージ（毒・やけど等）
    /// </summary>
    public List<AilmentDamage> AilmentDamages { get; set; } = new();
    
    /// <summary>
    /// ターン終了時のHP変化
    /// </summary>
    public List<HpChange> HpChanges { get; set; } = new();
    
    /// <summary>
    /// 強制交代（瀕死による自動交代）
    /// </summary>
    public List<ForcedSwitch> ForcedSwitches { get; set; } = new();
}
