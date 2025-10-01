using System.Collections.Generic;

namespace server.Models.Battle
{
    public class TurnResult
    {
        public List<string> Logs { get; set; } = new List<string>();
        public bool BattleEnded { get; set; } = false;
        public string? WinnerId { get; set; }
    }
}