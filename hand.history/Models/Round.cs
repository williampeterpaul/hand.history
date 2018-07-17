using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Models
{
    public class Round
    {
        public decimal Pot { get; set; }

        public StreetType Street { get; set; }
        public ICollection<Card> Community { get; }
        public ICollection<Action> Actions { get; set; }

        public enum StreetType
        {
            Preflop, Flop, Turn, River
        }
    }
}
