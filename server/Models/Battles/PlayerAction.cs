namespace server.Models.Battles
{
    public enum ActionType { Move, Switch, Catch };
    public class PlayerAction
    {
        public ActionType ActionType { get; set; }
        public int Value { get; set; }
    }
}