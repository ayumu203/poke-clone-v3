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

        // Create random wild Pokemon party for CPU (3 Pokemon)
        var allSpecies = await _pokemonSpeciesRepository.GetAllAsync();
        if (allSpecies == null || !allSpecies.Any())
        {
            throw new InvalidOperationException("No Pokemon species data found. Please seed the database first.");
        }

        var speciesList = allSpecies.ToList();
        var random = new Random();
        var cpuParty = new List<Pokemon>();

        // 3体のランダムなポケモンを生成（レベル5～8）
        for (int i = 0; i < 3; i++)
        {
            var randomSpecies = speciesList[random.Next(speciesList.Count)];
            var level = random.Next(5, 9); // 5～8レベル
            var wildPokemon = new Pokemon
            {
                PokemonId = Guid.NewGuid().ToString(),
                Species = randomSpecies,
                Level = level,
                Exp = 0,
                Moves = randomSpecies.MoveList.Take(4).ToList()
            };
            cpuParty.Add(wildPokemon);
        }

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

            // 捕獲アクションの場合、パーティサイズをチェック
            if (p1Action.ActionType == Domain.Enums.ActionType.Catch)
            {
                var isPartyFull = await _pokemonRepository.IsPartyFullAsync(battleState.Player1.PlayerId);
                if (isPartyFull)
                {
                    // パーティが満杯の場合、捕獲を失敗させる
                    var failedResult = new ProcessResult();
                    failedResult.ActionResults.Add(new ActionResult
                    {
                        ActionPokemonId = battleState.Player1.PokemonEntities[battleState.Player1.ActivePokemonIndex].PokemonId,
                        ActionType = Domain.Enums.ActionType.Catch,
                        CatchResult = new CatchResult
                        {
                            IsSuccess = false,
                            CaughtPokemonId = string.Empty
                        }
                    });
                    return failedResult;
                }
            }

            if (p2Action.ActionType == Domain.Enums.ActionType.Catch)
            {
                var isPartyFull = await _pokemonRepository.IsPartyFullAsync(battleState.Player2.PlayerId);
                if (isPartyFull)
                {
                    // パーティが満杯の場合、捕獲を失敗させる
                    var failedResult = new ProcessResult();
                    failedResult.ActionResults.Add(new ActionResult
                    {
                        ActionPokemonId = battleState.Player2.PokemonEntities[battleState.Player2.ActivePokemonIndex].PokemonId,
                        ActionType = Domain.Enums.ActionType.Catch,
                        CatchResult = new CatchResult
                        {
                            IsSuccess = false,
                            CaughtPokemonId = string.Empty
                        }
                    });
                    return failedResult;
                }
            }

            var result = battle.ProcessTurn(p1Action, p2Action);

            // ダメージ・回復・状態異常をHPに反映
            ApplyTurnEffects(result, battleState);

            // HP変化を記録
            RecordHpChanges(result, battleState);

            // 瀕死ポケモンの自動交代
            ProcessFaintedSwitch(result, battleState);

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
    /// ターン処理結果に基づいてダメージ、回復、状態異常をHPに反映する
    /// </summary>
    private void ApplyTurnEffects(ProcessResult result, BattleState battleState)
    {
        foreach (var actionResult in result.ActionResults)
        {
            if (actionResult.MoveResult == null) continue;

            var mr = actionResult.MoveResult;
            var targetId = mr.TargetId;
            var attackerId = actionResult.ActionPokemonId;

            // ダメージ適用
            if (mr.Damage > 0)
            {
                ApplyDamageToTarget(targetId, mr.Damage, battleState);
            }

            // ドレイン（攻撃者の回復）
            if (mr.Drain > 0)
            {
                ApplyHealingToTarget(attackerId, mr.Drain, battleState);
            }

            // 回復
            if (mr.Healing > 0)
            {
                ApplyHealingToTarget(attackerId, mr.Healing, battleState);
            }

            // 状態異常の適用
            if (mr.Ailment != null && mr.Ailment != Domain.Enums.Ailment.None)
            {
                ApplyAilmentToTarget(targetId, mr.Ailment.Value, battleState);
            }
        }

        // ターン末の状態異常ダメージ（毒・やけど）
        ApplyAilmentDamage(result, battleState);
    }

    /// <summary>
    /// ターゲットにダメージを与える
    /// </summary>
    private void ApplyDamageToTarget(string targetId, int damage, BattleState battleState)
    {
        var player1Pokemon = battleState.Player1.Party.FirstOrDefault(p => p.PokemonId == targetId);
        if (player1Pokemon != null)
        {
            player1Pokemon.CurrentHp = Math.Max(0, player1Pokemon.CurrentHp - damage);
            return;
        }

        var player2Pokemon = battleState.Player2.Party.FirstOrDefault(p => p.PokemonId == targetId);
        if (player2Pokemon != null)
        {
            player2Pokemon.CurrentHp = Math.Max(0, player2Pokemon.CurrentHp - damage);
        }
    }

    /// <summary>
    /// ターゲットを回復する
    /// </summary>
    private void ApplyHealingToTarget(string targetId, int healing, BattleState battleState)
    {
        var player1Pokemon = battleState.Player1.Party.FirstOrDefault(p => p.PokemonId == targetId);
        if (player1Pokemon != null)
        {
            player1Pokemon.CurrentHp = Math.Min(player1Pokemon.MaxHp, player1Pokemon.CurrentHp + healing);
            return;
        }

        var player2Pokemon = battleState.Player2.Party.FirstOrDefault(p => p.PokemonId == targetId);
        if (player2Pokemon != null)
        {
            player2Pokemon.CurrentHp = Math.Min(player2Pokemon.MaxHp, player2Pokemon.CurrentHp + healing);
        }
    }

    /// <summary>
    /// ターゲットに状態異常を適用する
    /// </summary>
    private void ApplyAilmentToTarget(string targetId, Domain.Enums.Ailment ailment, BattleState battleState)
    {
        var player1Pokemon = battleState.Player1.Party.FirstOrDefault(p => p.PokemonId == targetId);
        if (player1Pokemon != null)
        {
            // 既に状態異常がない場合のみ適用
            if (player1Pokemon.Ailment == Domain.Enums.Ailment.None)
            {
                player1Pokemon.Ailment = ailment;
            }
            return;
        }

        var player2Pokemon = battleState.Player2.Party.FirstOrDefault(p => p.PokemonId == targetId);
        if (player2Pokemon != null)
        {
            if (player2Pokemon.Ailment == Domain.Enums.Ailment.None)
            {
                player2Pokemon.Ailment = ailment;
            }
        }
    }

    /// <summary>
    /// ターン末の状態異常ダメージを適用する（毒・やけど）
    /// </summary>
    private void ApplyAilmentDamage(ProcessResult result, BattleState battleState)
    {
        // Player1のアクティブポケモン
        var p1Active = battleState.Player1.Party[battleState.Player1.ActivePokemonIndex];
        if (!p1Active.IsFainted)
        {
            ApplyAilmentDamageToOne(p1Active, result);
        }

        // Player2のアクティブポケモン
        var p2Active = battleState.Player2.Party[battleState.Player2.ActivePokemonIndex];
        if (!p2Active.IsFainted)
        {
            ApplyAilmentDamageToOne(p2Active, result);
        }
    }

    /// <summary>
    /// 1体のポケモンに状態異常ダメージを適用
    /// </summary>
    private void ApplyAilmentDamageToOne(PokemonState pokemon, ProcessResult result)
    {
        int damage = 0;

        switch (pokemon.Ailment)
        {
            case Domain.Enums.Ailment.Poison:
                // 通常毒: 最大HPの1/8ダメージ
                damage = Math.Max(1, pokemon.MaxHp / 8);
                break;

            case Domain.Enums.Ailment.DeadlyPoisoned:
                // もうどく: 最大HPの1/16 * ターン数（簡易実装: 1/8）
                damage = Math.Max(1, pokemon.MaxHp / 8);
                break;

            case Domain.Enums.Ailment.Burn:
                // やけど: 最大HPの1/16ダメージ
                damage = Math.Max(1, pokemon.MaxHp / 16);
                break;

            default:
                // 他の状態異常はダメージなし
                return;
        }

        if (damage > 0)
        {
            var hpBefore = pokemon.CurrentHp;
            pokemon.CurrentHp = Math.Max(0, pokemon.CurrentHp - damage);

            // 状態異常ダメージをProcessResultに記録
            result.AilmentDamages.Add(new AilmentDamage
            {
                PokemonId = pokemon.PokemonId,
                Ailment = pokemon.Ailment,
                Damage = damage,
                HpBefore = hpBefore,
                HpAfter = pokemon.CurrentHp
            });
        }
    }

    /// <summary>
    /// 全ポケモンのHP変化をProcessResultに記録
    /// </summary>
    private void RecordHpChanges(ProcessResult result, BattleState battleState)
    {
        // Player1のアクティブポケモン
        var p1Active = battleState.Player1.Party[battleState.Player1.ActivePokemonIndex];
        result.HpChanges.Add(new HpChange
        {
            PokemonId = p1Active.PokemonId,
            HpBefore = p1Active.CurrentHp, // 既に変化後の値だがログ用
            HpAfter = p1Active.CurrentHp,
            MaxHp = p1Active.MaxHp
        });

        // Player2のアクティブポケモン
        var p2Active = battleState.Player2.Party[battleState.Player2.ActivePokemonIndex];
        result.HpChanges.Add(new HpChange
        {
            PokemonId = p2Active.PokemonId,
            HpBefore = p2Active.CurrentHp,
            HpAfter = p2Active.CurrentHp,
            MaxHp = p2Active.MaxHp
        });
    }

    /// <summary>
    /// 瀕死ポケモンがいる場合、次のポケモンに自動交代
    /// </summary>
    private void ProcessFaintedSwitch(ProcessResult result, BattleState battleState)
    {
        // Player1の瀕死チェック
        ProcessFaintedSwitchForPlayer(battleState.Player1, result);

        // Player2の瀕死チェック
        ProcessFaintedSwitchForPlayer(battleState.Player2, result);
    }

    /// <summary>
    /// 1プレイヤーの瀕死ポケモンを次のポケモンに交代
    /// </summary>
    private void ProcessFaintedSwitchForPlayer(PlayerState playerState, ProcessResult result)
    {
        var activePokemon = playerState.Party[playerState.ActivePokemonIndex];
        
        if (!activePokemon.IsFainted)
        {
            return; // 瀕死でなければ何もしない
        }

        // 次の生きているポケモンを探す
        for (int i = 0; i < playerState.Party.Count; i++)
        {
            if (i != playerState.ActivePokemonIndex && !playerState.Party[i].IsFainted)
            {
                var faintedPokemonId = activePokemon.PokemonId;
                var nextPokemon = playerState.Party[i];

                // 交代実行
                playerState.ActivePokemonIndex = i;

                // 強制交代を記録
                result.ForcedSwitches.Add(new ForcedSwitch
                {
                    PlayerId = playerState.PlayerId,
                    FaintedPokemonId = faintedPokemonId,
                    NextPokemonId = nextPokemon.PokemonId
                });

                return;
            }
        }

        // 全滅している場合は何もしない（CheckBattleEndで処理）
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

        // EndResultを初期化
        var endResult = new EndResult
        {
            WinnerId = result.WinnerId,
            LoserId = loserState.PlayerId,
            Reason = Domain.Enums.EndReason.AllPokemonFainted,
            ExperienceGained = 0,
            Evolutions = new List<EvolutionInfo>()
        };

        // 捕獲判定
        var catchResult = result.ActionResults
            .FirstOrDefault(ar => ar.CatchResult?.IsSuccess == true);
        
        if (catchResult != null)
        {
            endResult.Reason = Domain.Enums.EndReason.Caught;
        }

        // 逃走判定
        var escapeResult = result.ActionResults
            .FirstOrDefault(ar => ar.EscapeResult?.IsSuccess == true);
        
        if (escapeResult != null)
        {
            endResult.Reason = Domain.Enums.EndReason.Escaped;
        }

        // CPUバトルの場合のみ経験値処理
        if (loserState.PlayerId != "CPU")
        {
            result.EndResult = endResult;
            return;
        }

        // 勝利したポケモンに経験値を加算
        var winnerPokemon = winnerState.PokemonEntities[winnerState.ActivePokemonIndex];
        var loserPokemon = loserState.PokemonEntities[loserState.ActivePokemonIndex];

        var expGain = _expCalculator.CalculateExpGain(loserPokemon.Level, winnerPokemon.Level);
        winnerPokemon.Exp += expGain;
        endResult.ExperienceGained = expGain;

        // 勝利報酬として所持金を加算
        var moneyReward = loserPokemon.Level * 100;
        var winnerPlayer = await _playerRepository.GetByIdAsync(winnerState.PlayerId);
        if (winnerPlayer != null)
        {
            winnerPlayer.Money += moneyReward;
            await _playerRepository.UpdateAsync(winnerPlayer);
            endResult.MoneyGained = moneyReward;
        }

        // レベルアップ判定
        var (newLevel, remainingExp) = _expCalculator.CalculateLevelUp(winnerPokemon.Exp, winnerPokemon.Level);
        
        if (newLevel > winnerPokemon.Level)
        {
            var oldSpeciesId = winnerPokemon.Species.PokemonSpeciesId;
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
                    
                    // 進化情報を記録
                    endResult.Evolutions.Add(new EvolutionInfo
                    {
                        PokemonId = winnerPokemon.PokemonId,
                        FromSpeciesId = oldSpeciesId,
                        ToSpeciesId = evolutionSpecies.PokemonSpeciesId
                    });
                }
            }
        }

        // DBへ保存
        await _pokemonRepository.UpdateAsync(winnerPokemon);

        // 捕獲処理
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

        // EndResultをProcessResultに設定
        result.EndResult = endResult;
    }
}
