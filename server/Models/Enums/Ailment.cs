namespace server.Models.Enums;

/// <summary>
/// 状態異常
/// </summary>
public enum Ailment
{
    None = 0,
    Paralysis = 1,    // まひ
    Sleep = 2,        // ねむり
    Freeze = 3,       // こおり
    Burn = 4,         // やけど
    Poison = 5,       // どく
    BadlyPoisoned = 6 // もうどく
}
