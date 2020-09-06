using CasinoGames.Website.HttpClients;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CasinoGames.Website.Controllers
{
    public class AdminController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Login([FromServices] IAuthHttpClient authClient)
        {
            await authClient.Login();
            return RedirectToAction(nameof(GameController.Index), "management");
        }
    }
}