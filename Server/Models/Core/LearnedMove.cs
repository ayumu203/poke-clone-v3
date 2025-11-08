using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models.Core;

/// <summary>
/// 覚えている技
/// </summary>
public class LearnedMove
{
    [Key]
    public int LearnedMoveId { get; set; }
    
    public int CurrentPP { get; set; }
    
    // 外部キー
    public int MoveId { get; set; }
    
    public int PokemonId { get; set; }
    
    // ナビゲーションプロパティ
    [ForeignKey(nameof(MoveId))]
    public Move Move { get; set; } = null!;
    
    [ForeignKey(nameof(PokemonId))]
    public Pokemon Pokemon { get; set; } = null!;
}
