namespace Server.Domain.Entities;

public class PlayerParty
{
    public int PlayerPartyId { get; set; }
    public string PlayerId { get; set; } = string.Empty;
    public Player? Player { get; set; }
    public List<Pokemon> Party { get; set; } = new();
}
