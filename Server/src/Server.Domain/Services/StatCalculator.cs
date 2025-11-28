namespace Server.Domain.Services;

public class StatCalculator : IStatCalculator
{
    public int CalcHp(int level, int baseStat)
    {
        // 第1世代のHP計算式: HP = floor(((Base + IV) * 2 + floor(sqrt(EV) / 4)) * Level / 100) + Level + 10
        // ここでは個体値15（最大値）、努力値65535（最大値）を仮定
        const int iv = 15;
        const int ev = 65535;
        var evBonus = (int)Math.Floor(Math.Ceiling(Math.Sqrt(ev)) / 4.0);
        return ((baseStat + iv) * 2 + evBonus) * level / 100 + level + 10;
    }

    public int CalcStat(int level, int baseStat)
    {
        return (baseStat * 2 * level) / 100 + 5;
    }

    public int CalcSpeed(int level, int baseStat)
    {
        return CalcStat(level, baseStat);
    }
}
