using Server.Domain.Enums;
using Server.Domain.Services;
using Xunit;

namespace Server.Tests.Domain.Services;

public class TypeEffectivenessManagerTests
{
    private readonly TypeEffectivenessManager _manager;

    public TypeEffectivenessManagerTests()
    {
        _manager = new TypeEffectivenessManager();
    }

    [Fact]
    public void GetEffectiveness_WaterToFire_Returns2()
    {
        var effectiveness = _manager.GetEffectiveness(PokemonType.Water, PokemonType.Fire);
        Assert.Equal(2.0, effectiveness);
    }

    [Fact]
    public void GetEffectiveness_FireToWater_Returns05()
    {
        var effectiveness = _manager.GetEffectiveness(PokemonType.Fire, PokemonType.Water);
        Assert.Equal(0.5, effectiveness);
    }

    [Fact]
    public void GetEffectiveness_ElectricToGround_Returns0()
    {
        var effectiveness = _manager.GetEffectiveness(PokemonType.Electric, PokemonType.Ground);
        Assert.Equal(0.0, effectiveness);
    }

    [Fact]
    public void GetEffectiveness_NormalToNormal_Returns1()
    {
        var effectiveness = _manager.GetEffectiveness(PokemonType.Normal, PokemonType.Normal);
        Assert.Equal(1.0, effectiveness);
    }
}
