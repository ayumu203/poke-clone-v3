namespace Server.Domain.Services;

public interface IRewardCalculator
{
    int CalculateMoneyReward(int loserLevel);
}
