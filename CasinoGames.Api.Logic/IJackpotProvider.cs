using CasinoGames.Shared.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CasinoGames.Api.Logic
{
    public interface IJackpotProvider
    {
        Task<IEnumerable<Game>> GetGamesAsync(CancellationToken cancellationToken = default);

        Task<Game> GetGameAsync(int id, CancellationToken cancellationToken = default);

        Task AddStatisticAsync(Game game, string sessionId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Jackpot>> GetJackpotsAsync(CancellationToken cancellationToken = default);
    }
}