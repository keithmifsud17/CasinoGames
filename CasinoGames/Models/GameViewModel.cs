using System.ComponentModel.DataAnnotations;

namespace CasinoGames.Website.Models
{
    public class GameViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required, Url]
        public string Image { get; set; }
        [Required, Url]
        public string Thumbnail { get; set; }
    }
}
