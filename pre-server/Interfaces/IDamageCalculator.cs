using server.Models;

namespace server.Interfaces
{
    public interface IDamageCalculator
    {
        int CalculateDamage(Pokemon attacker, Pokemon defender, Move move);
    }
}