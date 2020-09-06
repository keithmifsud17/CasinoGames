using CasinoGames.Shared.Models;
using System.Threading;
using System.Threading.Tasks;

namespace CasinoGames.Api.Logic
{
    public interface IAdminJackpotProvider
    {
        Task<Game> AddGame(string name, string image, string thumbnail, CancellationToken cancellationToken = default);

        Task DeleteGame(int id, CancellationToken cancellationToken = default);
    }
}