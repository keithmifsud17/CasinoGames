using CasinoGames.Website.HttpClients;
using CasinoGames.Website.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CasinoGames.Website.Controllers
{
    [Route("management/{action=Index}/{id?}")]
    public class GameController : Controller
    {
        public async Task<IActionResult> Index([FromServices] IAuthHttpClient authClient, [FromServices] IGameHttpClient gameClient)
        {
            var user = await authClient.Info();
            if (string.IsNullOrEmpty(user))
            {
                return View("Login");
            }
            return View(await gameClient.ListGamesAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromServices] IGameHttpClient gameClient, [FromForm] GameViewModel game)
        {
            try
            {
                await gameClient.AddGameAsync(game);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}