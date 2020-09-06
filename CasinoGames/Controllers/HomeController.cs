using CasinoGames.Api.HttpClients;
using CasinoGames.Models;
using CasinoGames.Shared.Models;
using CasinoGames.Website.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace CasinoGames.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index([FromServices] IGameHttpClient client)
        {
            var (games, jackpot) = await GetGamesAndJackpots(client);

            var model = new LobbyViewModel
            {
                TopGames = OrderWeighted(games)?.Take(6),
                LatestGame = games?.OrderByDescending(x => x.DateCreated)?.FirstOrDefault(),
                TopJackpots = jackpot?.OrderByDescending(x => x.Value)?.Take(3),
                TotalJackpot = jackpot?.Sum(x => x.Value) ?? default,
            };

            return View(model);
        }

        [HttpGet(nameof(Games), Name = nameof(Games))]
        public async Task<IActionResult> Games([FromServices] IGameHttpClient client)
        {
            return View(OrderWeighted(await client.ListGamesAsync()));
        }

        [HttpGet(nameof(Jackpots), Name = nameof(Jackpots))]
        public async Task<IActionResult> Jackpots([FromServices] IGameHttpClient client)
        {
            return View((await client.ListJackpotsAsync())?.OrderByDescending(jackpot => jackpot.Value));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IEnumerable<Game> OrderWeighted(IEnumerable<Game> games)
        {
            if (games is null)
            {
                return null;
            }

            return games
                .Select(game =>
                {
                    return new
                    {
                        Game = game,
                        Weight = (game.DateCreated - DateTime.UtcNow).TotalHours + game.TotalPlays // Negative weight for old games to push new games
                    };
                })
                .OrderByDescending(g => g.Weight)
                .Select(g => g.Game);
        }

        private async Task<(IEnumerable<Game>, IEnumerable<Jackpot>)> GetGamesAndJackpots(IGameHttpClient client)
        {
            var gamesTask = client.ListGamesAsync();
            var jackpotTask = client.ListJackpotsAsync();

            await Task.WhenAll(gamesTask, jackpotTask);

            // Tasks are already awaited so .Result is fine
            return (gamesTask.Result, jackpotTask.Result);
        }
    }
}