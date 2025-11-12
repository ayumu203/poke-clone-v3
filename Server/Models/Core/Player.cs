using System.ComponentModel.DataAnnotations;

namespace server.Models.Core;

/// <summary>
/// プレイヤー
/// </summary>
public class Player
{
    [Key]
    public string PlayerId { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    public string IconUrl { get; set; } = string.Empty;
    
    // ナビゲーションプロパティ
    public ICollection<Pokemon> Pokemons { get; set; } = new List<Pokemon>();
}
