namespace server.Interfaces
{
    public interface ITypeEffectivenessManager
    {
        double GetMuliplier(string attackType, string defenseType1, string? defenseType2 = null);
    }
}