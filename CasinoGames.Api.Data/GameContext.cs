using CasinoGames.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace CasinoGames.Api.Data
{
    public abstract class GameContext : DbContext
    {
        public GameContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<Jackpot> Jackpots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Statistic>().ToTable("Statistics");
            modelBuilder.Entity<Jackpot>().ToTable("Jackpots").Property(p => p.Value).HasColumnType("money");
        }
    }
}