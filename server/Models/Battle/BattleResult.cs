using System.Collections.Generic;

namespace server.Models.Battle
{
    public class BattleResult
    {
        public string WinnerId { get; }
        public string LoserId { get; }
        public List<string> FinalLogs { get; } = new List<string>();

        public BattleResult(string winnerId, string loserId, List<string> finalLogs)
        {
            WinnerId = winnerId;
            LoserId = loserId;
            FinalLogs = finalLogs;
        }
    }
}