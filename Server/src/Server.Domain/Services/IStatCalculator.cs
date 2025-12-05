namespace Server.Domain.Services;

public interface IStatCalculator
{
    int CalcHp(int level, int baseStat);
    int CalcStat(int level, int baseStat);
    int CalcSpeed(int level, int baseStat);
}
