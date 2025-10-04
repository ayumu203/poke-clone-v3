using server.Models.Battles;

namespace server.Models.DTOs
{
    public class BattleRoomDto
    {
        public string BattleRoomId { get; set; } = string.Empty;
        public PlayerDto Player1 { get; set; } = null!;
        public PlayerDto Player2 { get; set; } = null!;
        public Battle? Battle { get; set; }
    }
}