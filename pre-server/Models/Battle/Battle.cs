using server.Interfaces;
using System.Collections.Generic;

namespace server.Models.Battle
{
    public class Battle
    {
        public int Turn { get; private set; }
        public List<TurnResult> TurnResults { get; } = new List<TurnResult>();

        // ここはBattleRoomが値を持つ
        // public Player Player1 { get; }
        // public Player Player2 { get; }

        public int Player1ActivePokemonIndex { get; set; }
        public int Player2ActivePokemonIndex { get; set; }
        public List<Pokemon> Player1Party { get; }
        public List<Pokemon> Player2Party { get; }

        // ここはenumにしたい.  
        public string GameState { get; private set; } 

        private readonly IDamageCalculator _damageCalculator;
        private readonly ITypeEffectivenessManager _typeEffectivenessManager;

        // public Battle(Player player1, Player player2, List<Pokemon> player1Party, List<Pokemon> player2Party, IDamageCalculator damageCalculator)
        public Battle(List<Pokemon> player1Party, List<Pokemon> player2Party, IDamageCalculator damageCalculator, ITypeEffectivenessManager typeEffectivenessManager)
        {
            // Player1 = player1;
            // Player2 = player2;
            Player1Party = player1Party;
            Player2Party = player2Party;
            _damageCalculator = damageCalculator;
            _typeEffectivenessManager = typeEffectivenessManager;
            GameState = "WaitingForActions";
            Turn = 1;
        }

        public TurnResult ExecuteTurn(PlayerAction action1, PlayerAction action2)
        {
            // Main battle logic will go here
            // 1. Determine action order (priority, speed)
            // 2. Execute first action
            // 3. Check for fainted pokemon
            // 4. Execute second action
            // 5. Check for fainted pokemon
            // 6. End of turn effects (poison damage, etc.)
            // 7. Check for win/loss condition
            var result = new TurnResult();
            result.Logs.Add($"{Turn}ターン目の処理を開始します.");
            
            Turn++;
            return result;
        }
    }
}
