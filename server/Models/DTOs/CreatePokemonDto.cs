using System.Text.Json.Serialization;

namespace server.Models.DTOs;

public class CreatePokemonDto
{
    [JsonPropertyName("speciesId")]
    public int SpeciesId { get; set; }
    
    [JsonPropertyName("level")]
    public int Level { get; set; }
    
}