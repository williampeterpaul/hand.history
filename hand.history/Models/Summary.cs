using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static hand.history.Models.Round;

namespace hand.history.Models
{
    public class Summary
    {
        public decimal Id { get; }
        public string Title { get; set; }
        public string Game { get; set; }
        public string Currency { get; set; }

        public decimal Big { get; set; }
        public decimal Small { get; set; }
        public decimal TotalPot { get; set; }
        public decimal TotalRake { get; set; }

        public ICollection<Player> Players { get; }
        public ICollection<Round> Rounds { get; }

        public StreetType EndStreet => (StreetType)StreetCount;
        public int StreetCount => Rounds.GroupBy(x => x.Street).Count();
        public int RoundCount => Rounds.Count();
        public int PlayerCount => Players.Count();
    }
}
