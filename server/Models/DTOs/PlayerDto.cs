namespace server.Models.DTOs;

/// <summary>
/// プレイヤーデータのDTO
/// </summary>
public class PlayerDto
{
    public string PlayerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    
    // 手持ちポケモン（オプション）
    public List<PokemonDto>? Pokemons { get; set; }
}
