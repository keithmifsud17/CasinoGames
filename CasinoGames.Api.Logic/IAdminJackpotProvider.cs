using CasinoGames.Shared.Models;
using System.Threading;
using System.Threading.Tasks;

namespace CasinoGames.Api.Logic
{
    public interface IAdminJackpotProvider
    {
        Task<Game> AddGameAsync(string name, string image, string thumbnail, CancellationToken cancellationToken = default);

        Task DeleteGameAsync(int id, CancellationToken cancellationToken = default);
    }
}