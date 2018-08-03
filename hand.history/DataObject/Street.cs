using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.DataObject
{
    public class Street
    {
        public int Id { get; set; }
        public decimal Pot { get; set; }

        public StreetType Type { get; set; }
        public IEnumerable<Card> Community { get; set; }
        public IEnumerable<Action> Actions { get; set; }

        public enum StreetType
        {
            Preflop, Flop, Turn, River, Showdown
        }

        public override string ToString() => $"{Type}";
    }
}
