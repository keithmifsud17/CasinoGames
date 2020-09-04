using CasinoGames.Api.Logic;
using CasinoGames.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CasinoGames.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<Game>> Index([FromServices] IJackpotProvider provider)
        {
            return await provider.ListGames();
        }
    }
}