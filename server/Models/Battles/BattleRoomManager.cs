using server.Models.Basics;
using server.Interfaces;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace server.Models.Battles
{
    public class BattleRoomManager
    {
        private readonly ConcurrentDictionary<string, BattleRoom> _battleRooms = new();
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly CpuPlayer _cpuPlayer;

        public BattleRoomManager(IServiceScopeFactory scopeFactory, CpuPlayer cpuPlayer)
        {
            _cpuPlayer = cpuPlayer;
            _scopeFactory = scopeFactory;
        }

        public BattleRoom GetBattleRoom(string battleRoomId)
        {
            if (_battleRooms.TryGetValue(battleRoomId, out var battleRoom))
            {
                return battleRoom;
            }
            else
            {
                throw new KeyNotFoundException($"ルームID: {battleRoomId} のるーむは存在しません.");
            }
        }
        public async Task<BattleRoom> CreateCpuBattleRoom(Player player)
        {
            string battleRoomId = Guid.NewGuid().ToString();
            using (var scope = _scopeFactory.CreateScope())
            {
                var pokemonFactory = scope.ServiceProvider.GetRequiredService<IPokemonFactory>();
                List<Pokemon> player1Party = await pokemonFactory.MakePokemonListAsync(player);
                List<Pokemon> player2Party = await _cpuPlayer.MakeRandomPokemonList();
                var battle = new Battle(player1Party, player2Party);
                var battleRoom = new BattleRoom(battleRoomId, battle, player.PlayerId, _cpuPlayer.PlayerId);
                _battleRooms.TryAdd(battleRoom.BattleRoomId, battleRoom);
                return battleRoom;
            }
        }
        public void RemoveBattleRoom(string battleRoomId)
        {
            if (_battleRooms.ContainsKey(battleRoomId))
            {
                _battleRooms.TryRemove(battleRoomId, out _);
            }
        }
    }
}