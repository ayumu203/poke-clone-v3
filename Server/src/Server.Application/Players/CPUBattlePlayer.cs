using Server.Domain.Entities;
using Server.Domain.Enums;

namespace Server.Application.Players;

public class CPUBattlePlayer
{
    private readonly Random _random = new();

    public PlayerAction ChooseAction(BattlePlayer cpuPlayer)
    {
        var activePokemon = cpuPlayer.Party[cpuPlayer.ActivePokemonIndex];
        
        if (activePokemon.Moves.Count == 0)
        {
            // 技がない場合は交代
            return new PlayerAction
            {
                ActionType = ActionType.Switch,
                Value = _random.Next(cpuPlayer.Party.Count)
            };
        }

        // ランダムに技を選択
        var moveIndex = _random.Next(activePokemon.Moves.Count);
        var selectedMove = activePokemon.Moves[moveIndex];
        
        return new PlayerAction
        {
            ActionType = ActionType.Attack,
            Value = selectedMove.MoveId,
            PlayerId = cpuPlayer.Player.PlayerId
        };
    }

    public List<Pokemon> MakeRandomPokemonParty(List<PokemonSpecies> availableSpecies, int partySize = 6)
    {
        var party = new List<Pokemon>();
        
        for (int i = 0; i < partySize && i < availableSpecies.Count; i++)
        {
            var randomSpecies = availableSpecies[_random.Next(availableSpecies.Count)];
            var pokemon = new Pokemon
            {
                PokemonId = Guid.NewGuid().ToString(),
                Species = randomSpecies,
                Level = 50,
                Exp = 0,
                Moves = randomSpecies.MoveList.Take(4).ToList() // 最大4つの技
            };
            party.Add(pokemon);
        }

        return party;
    }
}
