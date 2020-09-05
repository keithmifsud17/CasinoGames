using CasinoGames.Shared.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CasinoGames.Api.Logic
{
    public interface IJackpotProvider
    {
        Task<IEnumerable<Game>> GetGames(CancellationToken cancellationToken = default);

        Task<Game> GetGame(int id, CancellationToken cancellationToken = default);

        Task AddStatistic(Game game, string sessionId, CancellationToken cancellationToken = default);
    }
}