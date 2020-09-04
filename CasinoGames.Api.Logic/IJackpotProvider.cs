using CasinoGames.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CasinoGames.Api.Logic
{
    public interface IJackpotProvider
    {
        Task<IEnumerable<Game>> ListGames();
    }
}