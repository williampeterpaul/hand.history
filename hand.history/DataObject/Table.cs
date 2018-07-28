using hand.history.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static hand.history.DataObject.Street;

namespace hand.history.DataObject
{
    public class Table
    {
        public double Id { get; set; }
        public double GameId { get; set; }
        public double TournamentId { get; set; }
        public double BBlind { get; set; }
        public double SBlind { get; set; }
        public double Pot { get; set; }
        public double Rake { get; set; }
        public string Currency { get; set; }
        public string Title { get; set; }
        public string Game { get; set; }
        public int Seats { get; set; }

        public DateTime Date { get; set; }

        public IEnumerable<Player> Players { get; }
        public IEnumerable<Street> Streets { get; }

        public int StreetCount => Streets.Count();
        public int PlayerCount => Players.Count();

        public StreetType EndStreet => (StreetType)StreetCount;
    }
}
