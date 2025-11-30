namespace Server.Domain.Entities;

public class EscapeResult
{
    public bool IsSuccess { get; set; }
    public string EscapingPlayerId { get; set; } = string.Empty;
    public string FailureReason { get; set; } = string.Empty;
}
