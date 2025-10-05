namespace server.Models.Battles.Core
{
    public enum ActionType { Move, Switch, Catch };
    public class PlayerAction
    {
        public ActionType ActionType { get; set; }
        public int Value { get; set; }
    }
}