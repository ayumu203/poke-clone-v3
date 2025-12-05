using Server.Domain.Services;

namespace Server.Infrastructure.Services;

public class ExpCalculator : IExpCalculator
{
    public int CalculateExpGain(int defeatedPokemonLevel, int victorPokemonLevel)
    {
        // 簡易的な経験値計算式
        // 基本経験値 = 敗北ポケモンのレベル * 50
        // レベル差による補正も考慮（高レベルポケモンを倒すと多くもらえる）
        var baseExp = defeatedPokemonLevel * 50;
        
        // レベルが低いプレイヤーはより多くの経験値を獲得
        if (victorPokemonLevel < defeatedPokemonLevel)
        {
            var levelDiff = defeatedPokemonLevel - victorPokemonLevel;
            baseExp += levelDiff * 10;
        }
        
        return baseExp;
    }

    public int CalculateExpToNextLevel(int currentLevel)
    {
        // 簡易的なレベルアップ必要経験値計算
        // 必要経験値 = currentLevel * 100
        // 実際のポケモンでは複雑な成長曲線があるが、ここでは線形
        return currentLevel * 100;
    }

    public (int newLevel, int remainingExp) CalculateLevelUp(int currentExp, int currentLevel)
    {
        var newLevel = currentLevel;
        var remainingExp = currentExp;

        // 複数レベルアップの可能性を考慮
        while (true)
        {
            var expToNext = CalculateExpToNextLevel(newLevel);
            
            if (remainingExp >= expToNext)
            {
                remainingExp -= expToNext;
                newLevel++;
                
                // レベル100で打ち止め
                if (newLevel >= 100)
                {
                    newLevel = 100;
                    remainingExp = 0;
                    break;
                }
            }
            else
            {
                break;
            }
        }

        return (newLevel, remainingExp);
    }
}
