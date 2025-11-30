namespace Server.Domain.Services;

public interface IExpCalculator
{
    int CalculateExpGain(int defeatedPokemonLevel, int victorPokemonLevel);
    int CalculateExpToNextLevel(int currentLevel);
    (int newLevel, int remainingExp) CalculateLevelUp(int currentExp, int currentLevel);
}
