namespace server.Models.Battles;

/// <summary>
/// バトルログ
/// </summary>
public class Log
{
    public string EventName { get; set; } = string.Empty;
    
    public string LogMessage { get; set; } = string.Empty;
}
