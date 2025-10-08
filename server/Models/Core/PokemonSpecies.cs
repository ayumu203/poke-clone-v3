using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using server.Models.Enums;

namespace server.Models.Core;

/// <summary>
/// ポケモンの種族データ
/// </summary>
public class PokemonSpecies
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int PokemonSpeciesId { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string FrontImage { get; set; } = string.Empty;
    
    public string BackImage { get; set; } = string.Empty;
    
    public Models.Enums.Type Type1 { get; set; }
    
    public Models.Enums.Type? Type2 { get; set; }
    
    public int EvolveLevel { get; set; }
    
    // 種族値
    public int BaseHP { get; set; }
    public int BaseAttack { get; set; }
    public int BaseDefense { get; set; }
    public int BaseSpecialAttack { get; set; }
    public int BaseSpecialDefense { get; set; }
    public int BaseSpeed { get; set; }
    
    // ナビゲーションプロパティ
    public ICollection<Pokemon> Pokemons { get; set; } = new List<Pokemon>();
    public ICollection<Move> MoveList { get; set; } = new List<Move>();
}
