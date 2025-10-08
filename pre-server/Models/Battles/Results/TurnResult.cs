namespace server.Models.Battles.Results
{
    public class TurnResult
    {
        public BattleEvent BattleEvent { get; set; } = BattleEvent.none;
        public List<string> Logs { get; set; } = new List<string>();
    }
}