using System;
using System.Collections.Generic;
using System.Text;

namespace CasinoGames.Shared.Models
{
    public class Statistic
    {
        public int StatisticId { get; set; }
        public string SessionId { get; set; }
        public DateTime DateTime { get; set; }
        public Game Game { get; set; }

    }
}
