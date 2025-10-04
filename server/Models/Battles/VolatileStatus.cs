using server.Interfaces;

namespace server.Models.Basics.Battle
{
    public class VolatileStatus : IVolatileStatus
    {
        public bool IsConfused { get; set; }
        public int ConfusionTurns { get; set; }
        public bool IsFlinched { get; set; }

        public VolatileStatus()
        {
            Reset();
        }

        public void Reset()
        {
            IsConfused = false;
            ConfusionTurns = 0;
            IsFlinched = false;
        }

        public void DecrementConfusion()
        {
            if (IsConfused && ConfusionTurns > 0)
            {
                ConfusionTurns--;
                if (ConfusionTurns == 0)
                {
                    IsConfused = false;
                }
            }
        }
    }
}