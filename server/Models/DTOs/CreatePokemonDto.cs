namespace server.Models.DTOs;

/// <summary>
/// ポケモン作成用DTO
/// </summary>
public class CreatePokemonDto
{
    public int PokemonSpeciesId { get; set; }
    public int Level { get; set; } = 5;
}