namespace server.Models.Enums;

/// <summary>
/// 状態異常の種類
/// </summary>
public enum Ailment
{
    None = 0,           // なし
    Paralysis = 1,      // まひ
    Sleep = 2,          // ねむり
    Freeze = 3,         // こおり
    Burn = 4,           // やけど
    Poison = 5,         // どく
    Confusion = 6,      // こんらん
    Infatuation = 7,    // メロメロ
    Trap = 8,           // バインド系（まきつく、しめつける等）
    Nightmare = 9,      // あくむ
    Torment = 10,       // いちゃもん
    Disable = 11,       // かなしばり
    Yawn = 12,          // あくび
    HealBlock = 13,     // かいふくふうじ
    NoTypeImmunity = 14,// ミラクルアイ
    LeechSeed = 15,     // やどりぎのタネ
    Embargo = 16,       // さしおさえ
    PerishSong = 17,    // ほろびのうた
    Ingrain = 18,       // ねをはる
    Silence = 19,       // さわぐ封じ
    TarShot = 20        // タールショット
}