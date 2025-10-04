namespace server.Interfaces
{
    public interface ITypeEffectivenessManager
    {
        double GetMuliplier(string attackerType, string moveType, string defenseType1, string? defenseType2 = null);
    }
}