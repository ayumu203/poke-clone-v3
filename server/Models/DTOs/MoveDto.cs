using server.Models.Core;

namespace server.Models.DTOs;

/// <summary>
/// 技データのDTO
/// </summary>
public class MoveDto
{
    public int MoveId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string DamageClass { get; set; } = string.Empty;
    public int? Power { get; set; }
    public int? Accuracy { get; set; }
    public int Pp { get; set; }
    public int Priority { get; set; }

    // ランク変化
    public Rank Rank { get; set; } = new();
    public string StatTarget { get; set; } = string.Empty;
    public int StatChance { get; set; }

    // 状態異常
    public string Ailment { get; set; } = string.Empty;
    public int AilmentChance { get; set; }

    // HP変動
    public int Healing { get; set; }
    public int Drain { get; set; }
}
