namespace server.Models.DTOs;

/// <summary>
/// プレイヤーデータのDTO（レスポンス用）
/// </summary>
public class PlayerDto
{
    public string PlayerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    // 手持ちポケモン（オプション）
    public List<PokemonDto>? Pokemons { get; set; }
}
