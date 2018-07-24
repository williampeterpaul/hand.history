using hand.history.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static hand.history.Models.Round;

namespace hand.history.Models
{
    public class Table
    {
        public double Id { get; set; }

        public double GameId { get; set; }
        public double TournamentId { get; set; }
        public string Title { get; set; }
        public string Game { get; set; }
        public string Currency { get; set; }
        public double Big { get; set; }
        public double Small { get; set; }
        public double TotalPot { get; set; }
        public double TotalRake { get; set; }
        public int Seats { get; set; }

        public DateTime Date { get; set; }

        public ICollection<Player> Players { get; }
        public ICollection<Round> Rounds { get; }

        public StreetType EndStreet => (StreetType)StreetCount;
        public int StreetCount => Rounds.GroupBy(x => x.Street).Count();
        public int RoundCount => Rounds.Count();
        public int PlayerCount => Players.Count();
    }
}
