using Server.Domain.Enums;

namespace Server.Domain.Entities;

/// <summary>
/// 対戦終了時の詳細情報
/// </summary>
public class EndResult
{
    /// <summary>
    /// 勝者のプレイヤーID
    /// </summary>
    public string WinnerId { get; set; } = string.Empty;

    /// <summary>
    /// 敗者のプレイヤーID
    /// </summary>
    public string LoserId { get; set; } = string.Empty;

    /// <summary>
    /// 対戦終了の理由
    /// </summary>
    public EndReason Reason { get; set; }

    /// <summary>
    /// 獲得した経験値
    /// </summary>
    public int ExperienceGained { get; set; }

    /// <summary>
    /// 進化したポケモンの情報
    /// </summary>
    public List<EvolutionInfo> Evolutions { get; set; } = new();
}

/// <summary>
/// ポケモンの進化情報
/// </summary>
public class EvolutionInfo
{
    /// <summary>
    /// 進化したポケモンのID
    /// </summary>
    public string PokemonId { get; set; } = string.Empty;

    /// <summary>
    /// 進化前のポケモン種族ID
    /// </summary>
    public int FromSpeciesId { get; set; }

    /// <summary>
    /// 進化後のポケモン種族ID
    /// </summary>
    public int ToSpeciesId { get; set; }
}
