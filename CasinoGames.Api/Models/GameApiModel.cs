using System.ComponentModel.DataAnnotations;

namespace CasinoGames.Api.Models
{
    public class GameApiModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Url]
        public string Image { get; set; }

        [Required]
        [Url]
        public string Thumbnail { get; set; }
    }
}