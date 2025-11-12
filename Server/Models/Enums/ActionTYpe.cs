namespace server.Models.Enums;

/// <summary>
/// プレイヤーの行動タイプ
/// </summary>
public enum ActionType
{
    Move = 1,    // 技を使う
    Switch = 2,  // ポケモンを交代
    Catch = 3    // ポケモンを捕獲
}
