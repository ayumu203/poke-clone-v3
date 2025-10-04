namespace server.Models.Battles
{
    public class BattleRoom
    {
        public string BattleRoomId { get; private set; } = null!;
        public Battle Battle { get; private set; } = null!;
        public string Player1Id { get; private set; } = null!;
        public string Player2Id { get; private set; } = null!;
        public BattleRoom(string battleRoomId, Battle battle, string player1Id, string player2Id)
        {
            BattleRoomId = battleRoomId;
            Battle = battle;
            Player1Id = player1Id;
            Player2Id = player2Id;
        }
    }
}