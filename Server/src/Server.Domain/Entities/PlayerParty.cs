namespace Server.Domain.Entities;

public class PlayerParty
{
    public string PlayerId { get; set; } = string.Empty;
    public List<Pokemon> Party { get; set; } = new();
}
