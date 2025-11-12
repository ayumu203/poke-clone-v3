using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using server.Models.Enums;

namespace server.Models.Core;

/// <summary>
/// ポケモン個体データ
/// </summary>
public class Pokemon
{
    [Key]
    public int PokemonId { get; set; }
    
    public int Level { get; set; } = 1;
    
    public int Exp { get; set; } = 0;
    
    public int CurrentHP { get; set; }
    
    public Ailment Ailment { get; set; } = Ailment.None;
    
    // TODO: IVolatileStatusの実装は後で追加
    // public IVolatileStatus VolatileStatus { get; set; }
    
    // 外部キー
    public int PokemonSpeciesId { get; set; }
    
    public string PlayerId { get; set; } = string.Empty;
    
    // ナビゲーションプロパティ
    [ForeignKey(nameof(PokemonSpeciesId))]
    public PokemonSpecies Species { get; set; } = null!;
    
    [ForeignKey(nameof(PlayerId))]
    public Player Owner { get; set; } = null!;
    
    public Rank Rank { get; set; } = new Rank();
    
    public ICollection<LearnedMove> LearnedMoves { get; set; } = new List<LearnedMove>();
    
    // メソッド
    public string LevelUp()
    {
        Level++;
        return $"{Species.Name} is now level {Level}!";
    }
}
