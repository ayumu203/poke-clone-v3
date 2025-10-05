using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using server.Models.Battles.Core;
using server.Models.Battles.Services;
using server.Models.Battles.Players;
using server.Models.DTOs;
using System.Security.Claims;
using server.Models.Core;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BattleController : ControllerBase
    {
        private readonly BattleRoomManager _battleManager;
        private readonly CpuPlayer _cpuPlayer;

        public BattleController(BattleRoomManager battleManager, CpuPlayer cpuPlayer)
        {
            _battleManager = battleManager;
            _cpuPlayer = cpuPlayer;
        }

        [HttpPost("cpu")]
        public async Task<IActionResult> CreateCpuBattle()
        {
            var playerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var playerName = User.FindFirst(ClaimTypes.Name)?.Value;
            var playerIcon = User.FindFirst("IconUrl")?.Value;

            if (string.IsNullOrEmpty(playerId) || string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(playerIcon))
            {
                return Unauthorized("有効なユーザー情報が見つかりません。");
            }

            var player = new Player
            {
                PlayerId = playerId,
                Name = playerName,
                IconUrl = playerIcon
            };

            try
            {
                BattleRoom battleRoom = await _battleManager.CreateCpuBattleRoom(player);

                var battleRoomDto = new BattleRoomDto
                {
                    BattleRoomId = battleRoom.BattleRoomId,
                    Battle = battleRoom.Battle,
                    Player1 = new PlayerDto
                    {
                        PlayerId = player.PlayerId,
                        Name = player.Name,
                        IconUrl = player.IconUrl
                    },
                    Player2 = new PlayerDto
                    {
                        PlayerId = _cpuPlayer.PlayerId,
                        Name = _cpuPlayer.Name,
                        IconUrl = _cpuPlayer.IconUrl
                    }
                };

                return Ok(battleRoomDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"対戦ルームの作成中にエラーが発生しました: {ex.Message}");
            }
        }
    }
}