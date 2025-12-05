namespace Server.Domain.Enums;

/// <summary>
/// 対戦終了の理由
/// </summary>
public enum EndReason
{
    /// <summary>
    /// すべてのポケモンが倒れた
    /// </summary>
    AllPokemonFainted,

    /// <summary>
    /// ポケモンを捕獲した
    /// </summary>
    Caught,

    /// <summary>
    /// 逃走した
    /// </summary>
    Escaped
}
