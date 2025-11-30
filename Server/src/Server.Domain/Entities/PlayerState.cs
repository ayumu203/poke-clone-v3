namespace Server.Domain.Entities;

/// <summary>
/// バトル中のプレイヤーの状態を管理するクラス
/// </summary>
public class PlayerState
{
    /// <summary>
    /// プレイヤーID
    /// </summary>
    public string PlayerId { get; set; } = string.Empty;

    /// <summary>
    /// プレイヤー情報
    /// </summary>
    public Player Player { get; set; } = new();

    /// <summary>
    /// アクティブなポケモンのインデックス
    /// </summary>
    public int ActivePokemonIndex { get; set; }

    /// <summary>
    /// パーティのポケモン状態リスト
    /// </summary>
    public List<PokemonState> Party { get; set; } = new();

    /// <summary>
    /// パーティのポケモンエンティティリスト（Battle計算用）
    /// </summary>
    public List<Pokemon> PokemonEntities { get; set; } = new();

    /// <summary>
    /// プレイヤーのアクション
    /// </summary>
    public PlayerAction? Action { get; set; }

    /// <summary>
    /// 全てのポケモンが瀕死かどうか
    /// </summary>
    public bool AllPokemonFainted => Party.All(p => p.IsFainted);

    /// <summary>
    /// 現在アクティブなポケモンの状態
    /// </summary>
    public PokemonState ActivePokemon => Party[ActivePokemonIndex];
}
