using Server.Domain.Enums;

namespace Server.Domain.Entities;

public class PlayerAction
{
    public ActionType ActionType { get; set; }
    public int Value { get; set; }
}
