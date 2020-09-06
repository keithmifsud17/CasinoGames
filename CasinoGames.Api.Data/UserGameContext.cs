using CasinoGames.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace CasinoGames.Api.Data
{
    public class UserGameContext : GameContext
    {
        public UserGameContext(DbContextOptions<UserGameContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>().ToView("GamesView").Property(x => x.Url).HasComputedColumnSql("CONCAT('https://localhost:5001/api/game/play/',[gameId]");
            base.OnModelCreating(modelBuilder);
        }
    }
}