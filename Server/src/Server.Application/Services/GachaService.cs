using Server.Domain.Entities;
using Server.Domain.Repositories;
using Server.Domain.Services;

namespace Server.Application.Services;

public class GachaService : IGachaService
{
    private readonly IPokemonSpeciesRepository _pokemonSpeciesRepository;
    private readonly IPokemonRepository _pokemonRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IRandomProvider _randomProvider;
    
    private const int GachaCost = 5000;
    private const int MinGachaLevel = 1;
    private const int MaxGachaLevel = 11; // Next()は上限を含まないため11

    public GachaService(
        IPokemonSpeciesRepository pokemonSpeciesRepository,
        IPokemonRepository pokemonRepository,
        IPlayerRepository playerRepository,
        IRandomProvider randomProvider)
    {
        _pokemonSpeciesRepository = pokemonSpeciesRepository;
        _pokemonRepository = pokemonRepository;
        _playerRepository = playerRepository;
        _randomProvider = randomProvider;
    }

    public async Task<Pokemon> ExecuteGachaAsync(string playerId)
    {
        // プレイヤー情報を取得
        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
        {
            throw new InvalidOperationException("プレイヤーが見つかりません");
        }

        // 所持金が足りるか確認
        if (player.Money < GachaCost)
        {
            throw new InvalidOperationException($"所持金が不足しています（必要: {GachaCost}円, 所持: {player.Money}円）");
        }

        // パーティが6体未満であることを確認
        var isPartyFull = await _pokemonRepository.IsPartyFullAsync(playerId);
        if (isPartyFull)
        {
            throw new InvalidOperationException("パーティが満杯です");
        }

        // ランダムなポケモンを生成
        var allSpecies = await _pokemonSpeciesRepository.GetAllAsync();
        if (allSpecies == null || allSpecies.Count == 0)
        {
            throw new InvalidOperationException("ポケモンデータが見つかりません");
        }

        var randomSpecies = allSpecies[_randomProvider.Next(allSpecies.Count)];
        var randomLevel = _randomProvider.Next(MinGachaLevel, MaxGachaLevel);

        var pokemon = new Pokemon
        {
            PokemonId = Guid.NewGuid().ToString(),
            Species = randomSpecies,
            Level = randomLevel,
            Exp = 0,
            Moves = randomSpecies.MoveList.Take(4).ToList()
        };

        // 所持金を減算
        player.Money -= GachaCost;
        await _playerRepository.UpdateAsync(player);

        // パーティに追加
        await _pokemonRepository.AddToPartyAsync(playerId, pokemon);

        return pokemon;
    }
}
