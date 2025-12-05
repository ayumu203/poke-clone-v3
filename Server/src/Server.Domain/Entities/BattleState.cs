namespace Server.Domain.Entities;

public class BattleState
{
    public string BattleId { get; set; } = string.Empty;
    public PlayerState Player1 { get; set; } = new();
    public PlayerState Player2 { get; set; } = new();
    public int Turn { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpireAt { get; set; }
}
