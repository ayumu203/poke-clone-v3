public class CreatePlayerDto
{
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    // PlayerId は含めない（JWTから取得する）
}