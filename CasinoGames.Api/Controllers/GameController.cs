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
        public async Task<IEnumerable<Game>> GetGamesAsync([FromServices] IJackpotProvider provider, CancellationToken cancellationToken)
        {
            return await provider.GetGamesAsync(cancellationToken);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetGameAsync([FromServices] IJackpotProvider provider, [FromRoute(Name = "id")] int gameId, CancellationToken cancellationToken)
        {
            var game = await provider.GetGameAsync(gameId, cancellationToken);
            if (game != default)
            {
                return Ok(game);
            }
            return NotFound();
        }

        [HttpGet("play/{id:int}")]
        public async Task<IActionResult> PlayGameAsync([FromServices] IJackpotProvider provider, [FromRoute] int id, CancellationToken cancellationToken)
        {
            var game = await provider.GetGameAsync(id, cancellationToken);
            if (game != default)
            {
                // sessionId should be something from a header
                await provider.AddStatisticAsync(game, Guid.NewGuid().ToString(), cancellationToken);
                return Ok($"Now playing {game.Name}");
            }
            return NotFound();
        }

        [HttpGet("jackpots")]
        public async Task<IEnumerable<Jackpot>> GetJackpotsAsync([FromServices] IJackpotProvider provider, CancellationToken cancellationToken)
        {
            return await provider.GetJackpotsAsync(cancellationToken);
        }

        [HttpPost, Authorize]
        public async Task<Game> AddGameAsync([FromServices] IAdminJackpotProvider provider, [FromBody] GameApiModel game, CancellationToken cancellationToken)
        {
            return await provider.AddGameAsync(game.Name, game.Image, game.Thumbnail, cancellationToken);
        }

        [HttpDelete("{id:int}"), Authorize]
        public async Task DeleteAsync([FromServices] IAdminJackpotProvider provider, [FromRoute(Name = "id")] int gameId, CancellationToken cancellationToken)
        {
            await provider.DeleteGameAsync(gameId, cancellationToken);
        }
    }
}