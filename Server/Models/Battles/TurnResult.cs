namespace server.Models.Battles;

/// <summary>
/// ターン実行結果
/// </summary>
public class TurnResult
{
    public List<Log> Logs { get; set; } = new List<Log>();
}
