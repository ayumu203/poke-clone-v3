namespace Server.Domain.Services;

public class RewardCalculator : IRewardCalculator
{
    private const int MoneyPerLevel = 100;

    public int CalculateMoneyReward(int loserLevel)
    {
        return loserLevel * MoneyPerLevel;
    }
}
