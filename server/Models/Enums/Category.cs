namespace server.Models.Enums;

/// <summary>
/// 技のカテゴリー
/// </summary>
public enum Category
{
    Damage = 0,           // ダメージ技
    Ailment = 1,          // 状態異常技
    NetGoodStats = 2,     // 能力変化（プラス）
    Heal = 3,             // 回復技
    DamageAilment = 4,    // ダメージ+状態異常
    Swagger = 5,          // いばる系
    DamageLower = 6,      // ダメージ+能力低下
    DamageRaise = 7,      // ダメージ+能力上昇
    DamageHeal = 8,       // ダメージ+回復
    OHKo = 9,             // 一撃必殺
    WholeFieldEffect = 10,// フィールド効果
    FieldEffect = 11,     // フィールド効果（単体）
    ForceSwitch = 12,     // 強制交代
    Unique = 13           // 特殊効果
}