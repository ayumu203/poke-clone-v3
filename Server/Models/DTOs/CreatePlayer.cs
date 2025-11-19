public class CreatePlayerDto
{
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public string PlayerId { get; set; } = string.Empty;
}