using server.Models.Battles.Results;

namespace server.Models.Battles.Results
{
    public class DamageResult
    {
        public BattleEvent BattleEvent { get; set; } = BattleEvent.damage;
        public int Damage { get; set; } = 0;
        public string Log { get; set; } = string.Empty;
    }
}