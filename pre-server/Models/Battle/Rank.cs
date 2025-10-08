using server.interfaces;
using System;

namespace server.Models.Battle
{
    public class Rank : IRank
    {
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int SpecialAttack { get; set; }
        public int SpecialDefense { get; set; }
        public int Speed { get; set; }
        public int Accuracy { get; set; }
        public int Evasion { get; set; }

        public void Reset()
        {
            Attack = 0;
            Defense = 0;
            SpecialAttack = 0;
            SpecialDefense = 0;
            Speed = 0;
            Accuracy = 0;
            Evasion = 0;
        }
    }
}