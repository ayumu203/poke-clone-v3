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

public class ProcessResult
{
    public List<ActionResult> ActionResults { get; set; } = new();
    public bool IsBattleEnd { get; set; }
    public string WinnerId { get; set; } = string.Empty;
    public EndResult? EndResult { get; set; }
}
