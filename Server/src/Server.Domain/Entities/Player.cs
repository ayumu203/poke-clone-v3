namespace Server.Domain.Entities;

public class Player
{
    public const int InitialMoney = 10000;
    
    public string PlayerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public int Money { get; set; } = InitialMoney;
}
