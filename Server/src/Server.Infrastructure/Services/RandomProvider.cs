namespace Server.Infrastructure.Services;

public class RandomProvider : Server.Domain.Services.IRandomProvider
{
    private readonly Random _random = new Random();

    public int Next(int minValue, int maxValue)
    {
        return _random.Next(minValue, maxValue);
    }

    public int Next(int maxValue)
    {
        return _random.Next(maxValue);
    }
}
