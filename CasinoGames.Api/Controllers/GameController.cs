using CasinoGames.Api.Logic;
using CasinoGames.Api.Models;
using CasinoGames.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CasinoGames.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<Game>> Index([FromServices] IJackpotProvider provider, CancellationToken cancellationToken)
        {
            return await provider.GetGames(cancellationToken);
        }

        [HttpGet("play/{id:int}")]
        public async Task<IActionResult> PlayGame([FromServices] IJackpotProvider provider, [FromRoute] int id, CancellationToken cancellationToken)
        {
            var game = await provider.GetGame(id, cancellationToken);
            if (game != default)
            {
                // sessionId should be something from a header
                await provider.AddStatistic(game, Guid.NewGuid().ToString(), cancellationToken);
                return Ok($"Now playing {game.Name}");
            }
            return NotFound();
        }

        [HttpGet("jackpots")]
        public async Task<IEnumerable<Jackpot>> Jackpots([FromServices] IJackpotProvider provider, CancellationToken cancellationToken)
        {
            return await provider.GetJackpots(cancellationToken);
        }

        [HttpPost, Authorize]
        public async Task<Game> AddGame([FromServices] IAdminJackpotProvider provider, [FromBody] GameApiModel game, CancellationToken cancellationToken)
        {
            return await provider.AddGame(game.Name, game.Image, game.Thumbnail, cancellationToken);
        }
    }
}