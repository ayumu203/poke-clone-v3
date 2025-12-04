namespace Server.Domain.Entities;

public class PlayerParty
{
    public int PlayerPartyId { get; set; }
    public string PlayerId { get; set; } = string.Empty;
    public List<Pokemon> Party { get; set; } = new();
}
