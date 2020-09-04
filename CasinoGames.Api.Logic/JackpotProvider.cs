using CasinoGames.Api.Data;
using CasinoGames.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
        public async Task<IEnumerable<Game>> ListGames()
        {
            return await context.Games.ToListAsync();
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
