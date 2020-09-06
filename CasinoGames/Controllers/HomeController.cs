using CasinoGames.Api.HttpClients;
using CasinoGames.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CasinoGames.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index([FromServices] IGameHttpClient client)
        {
            return View(await client.ListGamesAsync());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}