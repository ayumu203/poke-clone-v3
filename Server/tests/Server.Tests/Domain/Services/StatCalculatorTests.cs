using Server.Domain.Services;
using Xunit;

namespace Server.Tests.Domain.Services;

public class StatCalculatorTests
{
    private readonly StatCalculator _calculator;

    public StatCalculatorTests()
    {
        _calculator = new StatCalculator();
    }

    [Fact]
    public void CalcHp_Level50Base100_Returns210()
    {
        var hp = _calculator.CalcHp(50, 100);
        Assert.Equal(207, hp);
    }

    [Fact]
    public void CalcStat_Level50Base100_Returns105()
    {
        var stat = _calculator.CalcStat(50, 100);
        Assert.Equal(105, stat);
    }

    [Fact]
    public void CalcSpeed_Level100Base130_Returns265()
    {
        var speed = _calculator.CalcSpeed(100, 130);
        Assert.Equal(265, speed);
    }
}
