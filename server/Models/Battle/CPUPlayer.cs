namespace server.Models.Battle
{
    public class CPUPlayer : Player
    {
        public CPUPlayer()
        {
            PlayerId = "CPU";
            Name = "CPU Trainer";
            IconUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/5/53/Pok%C3%A9_Ball_icon.svg/768px-Pok%C3%A9_Ball_icon.svg.png?20161023215848";
        }

        public PlayerAction ChooseAction(Battle battle)
        {
            return new PlayerAction
            {
                Type = ActionType.Move,
                Value = 1
            };
        }
    }
}
