namespace server.Models.Core;

/// <summary>
/// ポケモンのランク補正値
/// </summary>
public class Rank
{
    public int Attack { get; set; } = 0;
    public int Defense { get; set; } = 0;
    public int SpecialAttack { get; set; } = 0;
    public int SpecialDefense { get; set; } = 0;
    public int Speed { get; set; } = 0;
    public int Accuracy { get; set; } = 0;
    public int Evasion { get; set; } = 0;
}
