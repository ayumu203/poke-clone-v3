namespace server.Models.Enums;

/// <summary>
/// バトルのゲーム状態
/// </summary>
public enum GameState
{
    WaitingForActions = 1,  // 行動待ち
    ProcessingTurn = 2,     // ターン処理中
    Finished = 3            // 終了
}
