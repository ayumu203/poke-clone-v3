using Server.Domain.Entities;
using Server.Domain.Enums;

namespace Server.Domain.Services;

public interface ICaptureCalculator
{
    bool CalculateCaptureSuccess(Pokemon targetPokemon, int currentHp, int maxHp, Ailment? ailment = null);
}
