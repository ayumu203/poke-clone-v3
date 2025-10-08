namespace server.Models.DTOs
{
    public class BattleRoomDto
    {
        public string RoomId { get; set; } = string.Empty;
        public PlayerDto Player1 { get; set; } = new PlayerDto();
        public PlayerDto Player2 { get; set; } = new PlayerDto();
        public string GameState { get; set; } = string.Empty;
    }
}
