namespace server.interfaces
{
    public interface IRank
    {
        int Attack { get; set; }
        int Defense { get; set; }
        int SpecialAttack { get; set; }
        int SpecialDefense { get; set; }
        int Speed { get; set; }
        int Accuracy { get; set; }
        int Evasion { get; set; }
        void Reset();
    }
}