using server.Models.Enums;

namespace server.Models.DTOs;

/// <summary>
/// ポケモン更新用DTO
/// </summary>
public class UpdatePokemonDto
{
    public int PokemonId { get; set; }
    public string? Nickname { get; set; }
    public int? CurrentHP { get; set; }
    public int? Experience { get; set; }
    public Ailment? Ailment { get; set; }
}