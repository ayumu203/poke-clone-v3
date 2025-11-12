using server.Models.Enums;

namespace server.Models.Battles;

/// <summary>
/// プレイヤーの行動
/// </summary>
public class PlayerAction
{
    public ActionType ActionType { get; set; }
    
    public int Value { get; set; }
}
