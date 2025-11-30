using Server.Domain.Enums;

namespace Server.Domain.Entities;

public class PlayerAction
{
    public string PlayerId { get; set; } = string.Empty;
    public ActionType ActionType { get; set; }
    public int Value { get; set; }
}
