using server.Models.Basics;
using server.Models.Battles.Result;
using server.Interfaces;

namespace server.Models.Battles
{
    public class Battle : ITypeEffectivenessManager
    {
        public int Turn { get; private set; } = 0;
        public List<TurnResult> TurnResults { get; private set; } = new List<TurnResult>();
        public Pokemon Player1ActivePokemon { get; private set; } = null!;
        public Pokemon Player2ActivePokemon { get; private set; } = null!;
        public List<Pokemon> Player1PartyList { get; private set; } = null!;
        public List<Pokemon> Player2PartyList { get; private set; } = null!;
        public GameState State { get; private set; } = GameState.WaitingForActions;
        /// <summary>
        /// 対戦の開始として呼び出す
        /// GameStateにより管理を行う.
        /// </summary>
        /// <returns></returns>
        public Battle StartBattle()
        {
            State = GameState.ProcessingTurn;
            return this;
        }
        /// <summary>
        /// ターンの処理を行う.
        /// </summary>
        /// <param name="player1Action"></param>
        /// <param name="player2Action"></param>
        /// <returns></returns>
        public List<TurnResult> ExecuteTurn(PlayerAction player1Action, PlayerAction player2Action)
        {
            // Todo: 対戦処理の実装
            Turn++;
            return new List<TurnResult>();
        }
        /// <summary>
        /// タイプ相性チャート
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, double>> typeChart = new Dictionary<string, Dictionary<string, double>>
        {
            { "Fire", new Dictionary<string, double> { { "Grass", 2.0 }, { "Water", 0.5 }, { "Fire", 0.5 }, { "Rock", 0.5 } } },
            { "Water", new Dictionary<string, double> { { "Fire", 2.0 }, { "Grass", 0.5 }, { "Water", 0.5 }, { "Electric", 1.0 } } },
            { "Grass", new Dictionary<string, double> { { "Water", 2.0 }, { "Fire", 0.5 }, { "Grass", 0.5 }, { "Rock", 2.0 } } },
        };
        /// <summary>
        /// タイプ相性の倍率を取得するためのメソッド
        /// </summary>
        /// <param name="attackerType"></param>
        /// <param name="moveType"></param>
        /// <param name="defenseType1"></param>
        /// <param name="defenseType2"></param>
        /// <returns></returns>
        public double GetMuliplier(string attackerType, string moveType, string defenseType1, string? defenseType2 = null)
        {
            double multiplier = 1.0;

            if (typeChart.ContainsKey(moveType) && typeChart[moveType].ContainsKey(defenseType1))
            {
                multiplier *= typeChart[moveType][defenseType1];
            }

            if (defenseType2 != null && typeChart.ContainsKey(moveType) && typeChart[moveType].ContainsKey(defenseType2))
            {
                multiplier *= typeChart[moveType][defenseType2];
            }

            return multiplier;
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="player1Party"></param>
        /// <param name="player2Party"></param>
        public Battle(List<Pokemon> player1Party, List<Pokemon> player2Party)
        {
            Player1PartyList = player1Party;
            Player2PartyList = player2Party;
            Player1ActivePokemon = player1Party.First();
            Player2ActivePokemon = player2Party.First();
        }
    }
}