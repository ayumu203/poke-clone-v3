using Server.Domain.Enums;

namespace Server.Domain.Entities;

/// <summary>
/// バトル中のポケモンの状態を管理するクラス
/// </summary>
public class PokemonState
{
    /// <summary>
    /// ポケモンID
    /// </summary>
    public string PokemonId { get; set; } = string.Empty;

    /// <summary>
    /// ポケモン種族ID
    /// </summary>
    public int PokemonSpeciesId { get; set; }

    /// <summary>
    /// 現在HP
    /// </summary>
    public int CurrentHp { get; set; }

    /// <summary>
    /// 最大HP
    /// </summary>
    public int MaxHp { get; set; }

    /// <summary>
    /// 状態異常
    /// </summary>
    public Ailment Ailment { get; set; } = Ailment.None;

    /// <summary>
    /// 能力ランク補正
    /// </summary>
    public Rank Rank { get; set; } = new();

    /// <summary>
    /// 瀕死状態かどうか
    /// </summary>
    public bool IsFainted => CurrentHp <= 0;

    /// <summary>
    /// PokemonエンティティからPokemonStateを作成
    /// </summary>
    public static PokemonState FromPokemon(Pokemon pokemon, int maxHp)
    {
        return new PokemonState
        {
            PokemonId = pokemon.PokemonId,
            PokemonSpeciesId = pokemon.Species.PokemonSpeciesId,
            CurrentHp = maxHp,
            MaxHp = maxHp,
            Ailment = Ailment.None,
            Rank = new Rank()
        };
    }
}
