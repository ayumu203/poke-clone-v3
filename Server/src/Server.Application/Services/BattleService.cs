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
    private readonly IPokemonRepository _pokemonRepository;
    private readonly IPlayerPartyRepository _playerPartyRepository;
    private readonly IExpCalculator _expCalculator;
    private readonly IEvolutionService _evolutionService;
    private readonly IPokemonSpeciesRepository _pokemonSpeciesRepository;

    public BattleService(
        IBattleRepository battleRepository,
        IPlayerRepository playerRepository,
        IDamageCalculator damageCalculator,
        ITypeEffectivenessManager typeEffectivenessManager,
        IStatCalculator statCalculator,
        IPokemonRepository pokemonRepository,
        IPlayerPartyRepository playerPartyRepository,
        IExpCalculator expCalculator,
        IEvolutionService evolutionService,
        IPokemonSpeciesRepository pokemonSpeciesRepository)
    {
        _battleRepository = battleRepository;
        _playerRepository = playerRepository;
        _damageCalculator = damageCalculator;
        _typeEffectivenessManager = typeEffectivenessManager;
        _statCalculator = statCalculator;
        _pokemonRepository = pokemonRepository;
        _playerPartyRepository = playerPartyRepository;
        _expCalculator = expCalculator;
        _evolutionService = evolutionService;
        _pokemonSpeciesRepository = pokemonSpeciesRepository;
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

        // Load player's party from database
        var playerParty = await _pokemonRepository.GetPlayerPartyAsync(playerId);
        if (playerParty == null || playerParty.Count == 0)
        {
            throw new InvalidOperationException("Player has no Pokemon in party. Please add Pokemon to your party first.");
        }

        // Create CPU player
        var cpuPlayer = new Player
        {
            PlayerId = "CPU",
            Name = "Wild Pokemon",
            IconUrl = string.Empty
        };

        // Create random wild Pokemon for CPU (using first available species)
        var allSpecies = await _pokemonSpeciesRepository.GetAllAsync();
        if (allSpecies == null || !allSpecies.Any())
        {
            throw new InvalidOperationException("No Pokemon species data found. Please seed the database first.");
        }

        var wildSpecies = allSpecies.First();
        var wildPokemon = new Pokemon
        {
            PokemonId = Guid.NewGuid().ToString(),
            Species = wildSpecies,
            Level = 5,
            Exp = 0,
            Moves = wildSpecies.MoveList.Take(4).ToList()
        };

        var cpuParty = new List<Pokemon> { wildPokemon };

        var battleState = new BattleState
        {
            BattleId = Guid.NewGuid().ToString(),
            Player1 = InitializePlayerState(player, playerParty),
            Player2 = InitializePlayerState(cpuPlayer, cpuParty),
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

            // Identify which action belongs to which player
            var p1Action = action1.PlayerId == battleState.Player1.PlayerId ? action1 : action2;
            var p2Action = action1.PlayerId == battleState.Player2.PlayerId ? action1 : action2;

            // Fallback if IDs don't match (should not happen with correct logic)
            if (p1Action.PlayerId != battleState.Player1.PlayerId) p1Action = action1;
            if (p2Action.PlayerId != battleState.Player2.PlayerId) p2Action = action2;

            var result = battle.ProcessTurn(p1Action, p2Action);

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

    /// <summary>
    /// バトル終了後処理：経験値加算、レベルアップ、進化処理
    /// </summary>
    public async Task ProcessPostBattleAsync(string battleId, ProcessResult result)
    {
        var battleState = await _battleRepository.GetAsync(battleId);
        if (battleState == null)
        {
            return;
        }

        // 勝者を判定
        if (!result.IsBattleEnd || string.IsNullOrEmpty(result.WinnerId))
        {
            return;
        }

        var winnerState = result.WinnerId == battleState.Player1.PlayerId
            ? battleState.Player1
            : battleState.Player2;
        var loserState = result.WinnerId == battleState.Player1.PlayerId
            ? battleState.Player2
            : battleState.Player1;

        // CPUバトルの場合のみ経験値処理
        if (loserState.PlayerId != "CPU")
        {
            return;
        }

        // 勝利したポケモンに経験値を加算
        var winnerPokemon = winnerState.PokemonEntities[winnerState.ActivePokemonIndex];
        var loserPokemon = loserState.PokemonEntities[loserState.ActivePokemonIndex];

        var expGain = _expCalculator.CalculateExpGain(loserPokemon.Level, winnerPokemon.Level);
        winnerPokemon.Exp += expGain;

        // レベルアップ判定
        var (newLevel, remainingExp) = _expCalculator.CalculateLevelUp(winnerPokemon.Exp, winnerPokemon.Level);
        
        if (newLevel > winnerPokemon.Level)
        {
            winnerPokemon.Level = newLevel;
            winnerPokemon.Exp = remainingExp;

            // 進化判定
            var canEvolve = await _evolutionService.CanEvolveAsync(
                winnerPokemon.Species.PokemonSpeciesId, newLevel);
            
            if (canEvolve)
            {
                var evolutionSpecies = await _evolutionService.GetEvolutionAsync(
                    winnerPokemon.Species.PokemonSpeciesId, newLevel);
                
                if (evolutionSpecies != null)
                {
                    winnerPokemon.Species = evolutionSpecies;
                }
            }
        }

        // DBへ保存
        await _pokemonRepository.UpdateAsync(winnerPokemon);

        // 捕獲処理
        var catchResult = result.ActionResults
            .FirstOrDefault(ar => ar.CatchResult?.IsSuccess == true);
        
        if (catchResult != null)
        {
            var caughtPokemonId = catchResult.CatchResult!.CaughtPokemonId;
            var caughtPokemon = loserState.Party.FirstOrDefault(p => p.PokemonId == caughtPokemonId);
            
            if (caughtPokemon != null)
            {
                // 野生ポケモンのデータから新しいPokemonエンティティを作成
                var caughtPokemonEntity = loserState.PokemonEntities
                    .FirstOrDefault(p => p.PokemonId == caughtPokemonId);
                
                if (caughtPokemonEntity != null)
                {
                    var isPartyFull = await _pokemonRepository.IsPartyFullAsync(winnerState.PlayerId);
                    if (!isPartyFull)
                    {
                        await _pokemonRepository.AddToPartyAsync(winnerState.PlayerId, caughtPokemonEntity);
                    }
                }
            }
        }
    }
}
