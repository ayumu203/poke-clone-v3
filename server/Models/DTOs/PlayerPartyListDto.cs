namespace server.Models.DTOs;

/// <summary>
/// プレイヤーの手持ちポケモンリストのDTO
/// </summary>
public class PlayerPartyListDto
{
    public List<PokemonDto> Pokemons { get; set; } = new();
}
