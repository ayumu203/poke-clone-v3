using server.Models.Core;

namespace server.Models.DTOs;

/// <summary>
/// 手持ちポケモンのDTO
/// </summary>
public class PokemonDto
{
    public int PokemonId { get; set; }
    public string Nickname { get; set; } = string.Empty;
    
    // 種族情報
    public PokemonSpeciesDto Species { get; set; } = null!;
    
    // レベル・経験値
    public int Level { get; set; }
    public int Exp { get; set; }
    
    // 現在のHP
    public int CurrentHp { get; set; }
    
    // 覚えている技
    public List<LearnedMove> LearnedMoves { get; set; } = new();
    
    // ランク変化
    public Rank Rank { get; set; } = new();
    
    // 状態異常
    public string Ailment { get; set; } = "None";
    
    // 揮発性状態 (混乱等)
    public string? VolatileStatus { get; set; }
}
