using System;

namespace Server.Models.Battle
{
    public class PlayerAction
    {
        public string ActionType { get; }
        public int Value { get; }

        public PlayerAction(string actionType, int value)
        {
            if (actionType != "move" || actionType != "switch")
            {
                throw new ArgumentException("無効な行動が選択されました.");
            }
            ActionType = actionType;
            Value = value;
        }
    }
}