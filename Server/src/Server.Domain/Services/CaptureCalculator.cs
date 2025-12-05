using Server.Domain.Entities;
using Server.Domain.Enums;

namespace Server.Domain.Services;

public class CaptureCalculator : ICaptureCalculator
{
    private readonly IRandomProvider _randomProvider;
    private const int BaseCaptureRate = 50;

    public CaptureCalculator(IRandomProvider randomProvider)
    {
        _randomProvider = randomProvider;
    }

    public bool CalculateCaptureSuccess(Pokemon targetPokemon, int currentHp, int maxHp, Ailment? ailment = null)
    {
        // 簡易的な実装: 将来的にHP、状態異常、ボールの種類を考慮可能
        var catchRate = _randomProvider.Next(0, 100);
        return catchRate < BaseCaptureRate;
    }
}
