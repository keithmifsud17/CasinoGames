using CasinoGames.Api.Data;
using CasinoGames.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CasinoGames.Api.Logic
{
    public abstract class JackpotProvider : IJackpotProvider
    {
        private readonly GameContext context;

        public JackpotProvider(GameContext context)
        {
            this.context = context;
        }

        public async Task AddStatistic(Game game, string sessionId, CancellationToken cancellationToken = default)
        {
            if (game is null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            if (sessionId is null)
            {
                throw new ArgumentNullException(nameof(sessionId));
            }

            if (!(await context.Games.ContainsAsync(game)))
            {
                throw new InvalidOperationException("Provided game does not exist", new ArgumentException(nameof(game)));
            }

            await context.Statistics.AddAsync(new Statistic { Game = game, SessionId = sessionId, DateTime = DateTime.UtcNow }, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Game> GetGame(int id, CancellationToken cancellationToken = default)
        {
            return await context.Games.FirstOrDefaultAsync(game => id.Equals(game.GameId), cancellationToken);
        }

        public async Task<IEnumerable<Game>> GetGames(CancellationToken cancellationToken = default)
        {
            return await context.Games.ToListAsync(cancellationToken);
        }
    }

    public class JackpotProviderA : JackpotProvider
    {
        public JackpotProviderA(GameContext context) : base(context)
        {
        }
    }

    public class JackpotProviderB : JackpotProvider
    {
        public JackpotProviderB(GameContext context) : base(context)
        {
        }
    }
}