namespace Server.Domain.Entities;

public class Rank
{
    public int Attack { get; set; }
    public int Defence { get; set; }
    public int SpecialAttack { get; set; }
    public int SpecialDefence { get; set; }
    public int Speed { get; set; }
    public int Accuracy { get; set; }
    public int Evasion { get; set; }

    public Rank()
    {
        Attack = 0;
        Defence = 0;
        SpecialAttack = 0;
        SpecialDefence = 0;
        Speed = 0;
        Accuracy = 0;
        Evasion = 0;
    }
}
