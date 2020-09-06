using CasinoGames.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace CasinoGames.Api.Data
{
    public class GameContext : DbContext
    {
        public GameContext(DbContextOptions<GameContext> options) : base(options)
        {
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Statistic> Statistics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>().ToTable("Games");
            modelBuilder.Entity<Statistic>().ToTable("Statistics");
        }
    }
}
