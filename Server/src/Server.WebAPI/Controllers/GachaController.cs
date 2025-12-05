using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Services;
using Server.Domain.Entities;
using System.Security.Claims;

namespace Server.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GachaController : ControllerBase
{
    private readonly IGachaService _gachaService;

    public GachaController(IGachaService gachaService)
    {
        _gachaService = gachaService;
    }

    /// <summary>
    /// ガチャを引く
    /// </summary>
    [HttpPost("pull")]
    public async Task<ActionResult<Pokemon>> PullGacha()
    {
        var playerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(playerId))
        {
            return StatusCode(500, "ユーザー情報の取得に失敗しました");
        }

        try
        {
            var pokemon = await _gachaService.ExecuteGachaAsync(playerId);
            return Ok(new 
            { 
                message = "ガチャを引きました",
                pokemon
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "ガチャの実行に失敗しました");
        }
    }
}
