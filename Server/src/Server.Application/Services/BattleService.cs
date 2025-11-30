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
            Player1 = InitializePlayerState(player1, new List<Pokemon>()),
            Player2 = InitializePlayerState(player2, new List<Pokemon>()),
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
            IconUrl = string.Empty
        };

        var battleState = new BattleState
        {
            BattleId = Guid.NewGuid().ToString(),
            Player1 = InitializePlayerState(player, new List<Pokemon>()),
            Player2 = InitializePlayerState(cpuPlayer, new List<Pokemon>()),
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

            // PlayerStateからBattlePlayerへ変換
            var battlePlayer1 = ConvertToBattlePlayer(battleState.Player1);
            var battlePlayer2 = ConvertToBattlePlayer(battleState.Player2);

            var battle = new Battle(
                _damageCalculator,
                _typeEffectivenessManager,
                _statCalculator,
                battlePlayer1,
                battlePlayer2);

            var result = battle.ProcessTurn(action1, action2);

            // ダメージをHPに反映
            ApplyDamage(result, battleState);

            // バトル終了判定
            CheckBattleEnd(result, battleState);

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

    /// <summary>
    /// PlayerとPokemonリストからPlayerStateを作成し、HPを初期化する
    /// </summary>
    private PlayerState InitializePlayerState(Player player, List<Pokemon> party)
    {
        var pokemonStates = party.Select(pokemon =>
        {
            var maxHp = _statCalculator.CalcHp(pokemon.Level, pokemon.Species.BaseHp);
            return PokemonState.FromPokemon(pokemon, maxHp);
        }).ToList();

        return new PlayerState
        {
            PlayerId = player.PlayerId,
            Player = player,
            ActivePokemonIndex = 0,
            Party = pokemonStates,
            PokemonEntities = party
        };
    }

    /// <summary>
    /// PlayerStateからBattlePlayerに変換する
    /// </summary>
    private BattlePlayer ConvertToBattlePlayer(PlayerState playerState)
    {
        return new BattlePlayer
        {
            Player = playerState.Player,
            Party = playerState.PokemonEntities,
            ActivePokemonIndex = playerState.ActivePokemonIndex
        };
    }

    /// <summary>
    /// ターン処理結果に基づいてダメージをHPに反映する
    /// </summary>
    private void ApplyDamage(ProcessResult result, BattleState battleState)
    {
        foreach (var actionResult in result.ActionResults)
        {
            if (actionResult.MoveResult != null && actionResult.MoveResult.Damage > 0)
            {
                // TargetIdを使って対象のポケモンを特定
                var targetId = actionResult.MoveResult.TargetId;
                
                // Player1のポケモンか確認
                var player1Pokemon = battleState.Player1.Party.FirstOrDefault(p => p.PokemonId == targetId);
                if (player1Pokemon != null)
                {
                    player1Pokemon.CurrentHp = Math.Max(0, player1Pokemon.CurrentHp - actionResult.MoveResult.Damage);
                    continue;
                }

                // Player2のポケモンか確認
                var player2Pokemon = battleState.Player2.Party.FirstOrDefault(p => p.PokemonId == targetId);
                if (player2Pokemon != null)
                {
                    player2Pokemon.CurrentHp = Math.Max(0, player2Pokemon.CurrentHp - actionResult.MoveResult.Damage);
                }
            }
        }
    }

    /// <summary>
    /// バトル終了判定を行う
    /// </summary>
    private void CheckBattleEnd(ProcessResult result, BattleState battleState)
    {
        if (battleState.Player1.AllPokemonFainted || battleState.Player2.AllPokemonFainted)
        {
            result.IsBattleEnd = true;
            result.WinnerId = battleState.Player1.AllPokemonFainted
                ? battleState.Player2.PlayerId
                : battleState.Player1.PlayerId;
        }
    }
}
