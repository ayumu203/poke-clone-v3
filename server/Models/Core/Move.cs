using System.ComponentModel.DataAnnotations;
using server.Models.Enums;

namespace server.Models.Core;

/// <summary>
/// 技データ
/// </summary>
public class Move
{
    [Key]
    public int MoveId { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public Models.Enums.Type Type { get; set; }
    
    public Category Category { get; set; }
    
    public DamageClass DamageClass { get; set; }
    
    public int Power { get; set; }
    
    public int Accuracy { get; set; }
    
    public int PP { get; set; }
    
    public int Priority { get; set; } = 0;
    
    public Rank Rank { get; set; } = new Rank();
    
    public string StatTarget { get; set; } = string.Empty;
    
    public int StatChance { get; set; } = 0;
    
    public Ailment Ailment { get; set; } = Ailment.None;
    
    public int AilmentChance { get; set; } = 0;
    
    public int Healing { get; set; } = 0;
    
    public int Drain { get; set; } = 0;
}
