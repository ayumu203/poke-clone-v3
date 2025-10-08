using System.Collections.Generic;

namespace server.Models.Battle
{
    public class BattleResult
    {
        public string WinnerId { get; } = string.Empty;
        public string LoserId { get; } = string.Empty;
        public List<string> Logs { get; set; } = new List<string>();
        public int totalTurns { get; set; }
    }
}