using server.Models.Battles.Result;

namespace server.Model.Battle.Result
{
    public class DamageResult
    {
        public BattleEvent BattleEvent { get; set; } = BattleEvent.damage;
        public int Damage { get; set; } = 0;
        public string Log { get; set; } = string.Empty;
    }
}