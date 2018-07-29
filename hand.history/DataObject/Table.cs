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
        public decimal Id { get; set; }
        public decimal HandId { get; set; }
        public decimal TournamentId { get; set; }
        public decimal BBlind { get; set; }
        public decimal SBlind { get; set; }
        public decimal Pot { get; set; }
        public decimal Rake { get; set; }
        public string Currency { get; set; }
        public string Title { get; set; }
        public string Game { get; set; }
        public int Seats { get; set; }
        public int SeatsMax { get; set; }

        public DateTime Date { get; set; }
        public IEnumerable<Player> Players { get; set; }
        public IEnumerable<Street> Streets { get; set; }

        public int StreetCount => Streets.Count();
        public int PlayerCount => Players.Count();

        public StreetType EndStreet => (StreetType)StreetCount;
    }
}
