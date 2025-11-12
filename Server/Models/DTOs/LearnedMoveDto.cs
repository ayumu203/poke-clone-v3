namespace server.Models.DTOs;

/// <summary>
/// 覚えている技のDTO
/// </summary>
public class LearnedMoveDto
{
    public int LearnedMoveId { get; set; }
    public int CurrentPP { get; set; }
    public MoveDto Move { get; set; } = null!;
}
