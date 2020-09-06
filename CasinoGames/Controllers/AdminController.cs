using CasinoGames.Website.HttpClients;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CasinoGames.Website.Controllers
{
    public class AdminController : Controller
    {
        [HttpPost, ActionName("Login")]
        public async Task<IActionResult> LoginAsync([FromServices] IAuthHttpClient authClient)
        {
            await authClient.LoginAsync();
            return RedirectToAction(nameof(GameController.Index), "management");
        }
    }
}