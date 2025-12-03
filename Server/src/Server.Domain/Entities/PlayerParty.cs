namespace Server.Domain.Entities;

public class PlayerParty
{
    public int PlayerPartyId { get; set; }
    public string PlayerId { get; set; } = string.Empty;
    // Player? Player { get; set; } を削除してEFの追跡問題を回避
    public List<Pokemon> Party { get; set; } = new();
}
