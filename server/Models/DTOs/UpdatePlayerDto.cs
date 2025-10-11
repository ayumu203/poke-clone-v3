namespace server.Models.DTOs;

/// <summary>
/// プレイヤー更新リクエスト用DTO
/// </summary>
public class UpdatePlayerDto
{
    // PlayerId は含めない（JWTから取得する）
    public required string Name { get; set; }
    public string? IconUrl { get; set; }
}
