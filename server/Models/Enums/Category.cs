namespace server.Models.Enums;

/// <summary>
/// 技のカテゴリ（効果分類）
/// </summary>
public enum Category
{
    Damage = 1,           // ダメージ
    Ailment = 2,          // 状態異常
    NetGoodStats = 3,     // 能力上昇
    Heal = 4,             // 回復
    DamageAilment = 5,    // ダメージ+状態異常
    Swagger = 6,          // いばる系
    DamageRaise = 7,      // ダメージ+能力上昇
    DamageLower = 8,      // ダメージ+能力低下
    InflictStatus = 9,    // 状態異常付与
    UniqueEffect = 10     // 特殊効果
}
