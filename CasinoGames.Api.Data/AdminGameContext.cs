using CasinoGames.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace CasinoGames.Api.Data
{
    public class AdminGameContext : GameContext
    {
        public AdminGameContext(DbContextOptions<AdminGameContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var games = modelBuilder.Entity<Game>().ToTable("Games");

            games.Property(x => x.Url).HasComputedColumnSql("CONCAT('https://localhost:5001/api/game/play/',[gameId]");
            games.Ignore(x => x.TotalPlays);

            base.OnModelCreating(modelBuilder);
        }
    }
}