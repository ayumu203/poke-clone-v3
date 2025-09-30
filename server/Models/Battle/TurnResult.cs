using System.Collections.Generic;

namespace server.Models.Battle
{
    public class TurnResult
    {
        public string EventType { get; }
        public List<string> Logs { get; } = new List<string>();

        public TurnResult(string eventType)
        {
            EventType = eventType;
        }
        public void AddLog(string log)
        {
            Logs.Add(log);
        }
    }
}