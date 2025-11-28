using Server.Domain;
using Server.Domain.Entities;
using Server.Domain.Repositories;
using Server.Domain.Services;

namespace Server.Application.Services;

public class BattleService : IBattleService
{
    private readonly IBattleRepository _battleRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IDamageCalculator _damageCalculator;
    private readonly ITypeEffectivenessManager _typeEffectivenessManager;
    private readonly IStatCalculator _statCalculator;

    public BattleService(
        IBattleRepository battleRepository,
        IPlayerRepository playerRepository,
        IDamageCalculator damageCalculator,
        ITypeEffectivenessManager typeEffectivenessManager,
        IStatCalculator statCalculator)
    {
        _battleRepository = battleRepository;
        _playerRepository = playerRepository;
        _damageCalculator = damageCalculator;
        _typeEffectivenessManager = typeEffectivenessManager;
        _statCalculator = statCalculator;
    }

    public async Task<BattleState> CreateBattleAsync(string player1Id, string player2Id)
    {
        var player1 = await _playerRepository.GetByIdAsync(player1Id);
        var player2 = await _playerRepository.GetByIdAsync(player2Id);

        if (player1 == null || player2 == null)
        {
            throw new InvalidOperationException("Player not found");
        }

        var battleState = new BattleState
        {
            BattleId = Guid.NewGuid().ToString(),
            Player1 = new BattlePlayer { Player = player1, Party = new List<Pokemon>(), ActivePokemonIndex = 0 },
            Player2 = new BattlePlayer { Player = player2, Party = new List<Pokemon>(), ActivePokemonIndex = 0 },
            Turn = 0,
            CreatedAt = DateTime.UtcNow,
            ExpireAt = DateTime.UtcNow.AddHours(1)
        };

        await _battleRepository.SaveAsync(battleState);
        return battleState;
    }

    public async Task<BattleState> CreateCpuBattleAsync(string playerId)
    {
        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
        {
            throw new InvalidOperationException("Player not found");
        }

        // Create CPU player and party (simplified for now)
        var cpuPlayer = new Player
        {
            PlayerId = "CPU",
            Name = "CPU Opponent",
            IconUrl = null
        };

        var battleState = new BattleState
        {
            BattleId = Guid.NewGuid().ToString(),
            Player1 = new BattlePlayer { Player = player, Party = new List<Pokemon>(), ActivePokemonIndex = 0 },
            Player2 = new BattlePlayer { Player = cpuPlayer, Party = new List<Pokemon>(), ActivePokemonIndex = 0 },
            Turn = 0,
            CreatedAt = DateTime.UtcNow,
            ExpireAt = DateTime.UtcNow.AddHours(1)
        };

        await _battleRepository.SaveAsync(battleState);
        return battleState;
    }

    public async Task<ProcessResult> ProcessTurnAsync(string battleId, PlayerAction action1, PlayerAction action2)
    {
        // Lock acquisition
        var lockAcquired = await _battleRepository.TryLockAsync(battleId, TimeSpan.FromSeconds(10));
        if (!lockAcquired)
        {
            throw new InvalidOperationException("Battle is currently being processed");
        }

        try
        {
            var battleState = await _battleRepository.GetAsync(battleId);
            if (battleState == null)
            {
                throw new InvalidOperationException("Battle not found");
            }

            var battle = new Battle(
                _damageCalculator,
                _typeEffectivenessManager,
                _statCalculator,
                battleState.Player1,
                battleState.Player2);

            var result = battle.ProcessTurn(action1, action2);

            battleState.Turn++;
            await _battleRepository.SaveAsync(battleState);

            return result;
        }
        finally
        {
            await _battleRepository.UnlockAsync(battleId);
        }
    }

    public async Task<BattleState?> GetBattleStateAsync(string battleId)
    {
        return await _battleRepository.GetAsync(battleId);
    }

    public async Task SaveBattleStateAsync(BattleState battleState)
    {
        await _battleRepository.SaveAsync(battleState);
    }

    public async Task DeleteBattleAsync(string battleId)
    {
        await _battleRepository.DeleteAsync(battleId);
    }
}
