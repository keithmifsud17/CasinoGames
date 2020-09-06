namespace CasinoGames.Shared.Models
{
    public class Jackpot
    {
        public int JackpotId { get; set; }
        public decimal Value { get; set; }
        public Game Game { get; set; }
    }
}