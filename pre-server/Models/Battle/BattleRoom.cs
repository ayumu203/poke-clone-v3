using server.Interfaces;
using System;

namespace server.Models.Battle
{
    public class BattleRoom
    {
        public string RoomId { get; set; }　= string.Empty;
        public string Player1Id { get; set; } = string.Empty;
        public PlayerParty Player1Party { get; set; } = new PlayerParty();
        public string Player2Id { get; set; } = string.Empty;
        public PlayerParty Player2Party { get; set; } = new PlayerParty();
    }
}

// 未実装メモ
// * パーティ作成のロジック
// * 追加ロジック
// * 対戦開始ロジック