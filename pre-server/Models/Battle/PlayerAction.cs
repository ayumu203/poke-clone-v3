namespace server.Models.Battle
{
    public enum ActionType { Move, Switch, Catch };
    public class PlayerAction
    {
        public ActionType Type { get; set; }
        public int Value { get; set; }
    }
}