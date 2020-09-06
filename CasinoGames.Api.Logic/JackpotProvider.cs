using CasinoGames.Api.Data;
using CasinoGames.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CasinoGames.Api.Logic
{
    public abstract class JackpotProvider : IJackpotProvider, IAdminJackpotProvider
    {
        private readonly UserGameContext context;
        private readonly AdminGameContext adminContext;

        public JackpotProvider(UserGameContext context, AdminGameContext adminContext)
        {
            this.context = context;
            this.adminContext = adminContext;
        }

        public async Task<Game> AddGame(string name, string image, string thumbnail, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (string.IsNullOrEmpty(thumbnail))
            {
                throw new ArgumentNullException(nameof(thumbnail));
            }
            if (string.IsNullOrEmpty(image))
            {
                throw new ArgumentNullException(nameof(image));
            }

            var newGameEntity = adminContext.Games.Attach(new Game
            {
                Name = name,
                Image = image,
                Thumbnail = thumbnail,
                DateCreated = DateTime.UtcNow,
            });

            await adminContext.SaveChangesAsync(cancellationToken);
            await newGameEntity.ReloadAsync();

            return newGameEntity.Entity;
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

        public async Task<IEnumerable<Jackpot>> GetJackpots(CancellationToken cancellationToken = default)
        {
            return await context.Jackpots.Include(a => a.Game).ToListAsync(cancellationToken);
        }
    }

    public class JackpotProviderA : JackpotProvider
    {
        public JackpotProviderA(UserGameContext context, AdminGameContext adminContext) : base(context, adminContext)
        {
        }
    }

    public class JackpotProviderB : JackpotProvider
    {
        public JackpotProviderB(UserGameContext context, AdminGameContext adminContext) : base(context, adminContext)
        {
        }
    }
}