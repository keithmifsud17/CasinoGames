using System;

namespace CasinoGames.Shared.Models
{
    public class Game
    {
        public int GameId { get; set; }
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public int TotalPlays { get; set; }
        public DateTime DateCreated { get; set; }
    }
}