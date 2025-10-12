namespace server.Models.DTOs;

/// <summary>
/// ポケモン種族データのDTO
/// </summary>
public class PokemonSpeciesDto
{
    public int PokemonSpeciesId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FrontImage { get; set; } = string.Empty;
    public string BackImage { get; set; } = string.Empty;
    public string Type1 { get; set; } = string.Empty;
    public string? Type2 { get; set; }
    public int EvolveLevel { get; set; }
    public int BaseHp { get; set; }
    public int BaseAttack { get; set; }
    public int BaseDefence { get; set; }
    public int BaseSpecialAttack { get; set; }
    public int BaseSpecialDefence { get; set; }
    public int BaseSpeed { get; set; }
    
    // 覚えられる技のリスト
    public List<MoveDto> MoveList { get; set; } = new();
}
