namespace Server.Domain.Entities;

public class Player
{
    public string PlayerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public int Money { get; set; } = 3000;
}
