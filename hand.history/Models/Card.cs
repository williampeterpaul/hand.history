using hand.history.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Models
{
    public class Card : IComparable<Card>
    {
        public Rank Rank { get; set; }
        public Suit Suit { get; set; }

        public int CompareTo(Card other)
        {
            if (Rank < other.Rank) return -1;
            if (Rank > other.Rank) return 1;

            return 0;
        }
    }
}
