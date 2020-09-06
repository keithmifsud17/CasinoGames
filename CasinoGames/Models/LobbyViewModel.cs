using CasinoGames.Shared.Models;
using System.Collections.Generic;

namespace CasinoGames.Website.Models
{
    public class LobbyViewModel
    {
        public IEnumerable<Game> TopGames { get; set; }
        public Game LatestGame { get; set; }

        public IEnumerable<Jackpot> TopJackpots { get; set; }
        public decimal TotalJackpot { get; set; }
    }
}