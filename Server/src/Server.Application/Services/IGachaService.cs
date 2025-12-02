using Server.Domain.Entities;

namespace Server.Application.Services;

public interface IGachaService
{
    Task<Pokemon> ExecuteGachaAsync(string playerId);
}
