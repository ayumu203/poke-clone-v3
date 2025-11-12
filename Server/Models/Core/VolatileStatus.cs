namespace server.Models.Core;

/// <summary>
/// 揮発性状態（バトル終了時に解除される状態異常）
/// </summary>
public class VolatileStatus
{
    /// <summary>
    /// 混乱状態かどうか
    /// </summary>
    public bool IsConfused { get; set; }
    
    /// <summary>
    /// 混乱状態の残りターン数
    /// </summary>
    public int ConfusionTurns { get; set; }
    
    /// <summary>
    /// ひるみ状態かどうか
    /// </summary>
    public bool IsFlinched { get; set; }
    
    /// <summary>
    /// 束縛状態かどうか（締め付ける、渦潮など）
    /// </summary>
    public bool IsTrapped { get; set; }
    
    /// <summary>
    /// 束縛状態の残りターン数
    /// </summary>
    public int TrappedTurns { get; set; }
    
    /// <summary>
    /// 宿り木状態かどうか
    /// </summary>
    public bool IsSeeded { get; set; }
    
    /// <summary>
    /// アンコール状態かどうか
    /// </summary>
    public bool IsEncored { get; set; }
    
    /// <summary>
    /// アンコール状態の残りターン数
    /// </summary>
    public int EncoreTurns { get; set; }
    
    /// <summary>
    /// アンコール中の技ID
    /// </summary>
    public int? EncoredMoveId { get; set; }
    
    /// <summary>
    /// 溜め状態かどうか（ソーラービーム、空を飛ぶなど）
    /// </summary>
    public bool IsCharging { get; set; }
    
    /// <summary>
    /// 溜め中の技ID
    /// </summary>
    public int? ChargingMoveId { get; set; }
    
    /// <summary>
    /// 防御状態かどうか（守る、見切りなど）
    /// </summary>
    public bool IsProtected { get; set; }
    
    /// <summary>
    /// 連続防御成功回数
    /// </summary>
    public int ProtectCount { get; set; }

    /// <summary>
    /// すべての揮発性状態をクリア
    /// </summary>
    public void Clear()
    {
        IsConfused = false;
        ConfusionTurns = 0;
        IsFlinched = false;
        IsTrapped = false;
        TrappedTurns = 0;
        IsSeeded = false;
        IsEncored = false;
        EncoreTurns = 0;
        EncoredMoveId = null;
        IsCharging = false;
        ChargingMoveId = null;
        IsProtected = false;
        ProtectCount = 0;
    }
}
