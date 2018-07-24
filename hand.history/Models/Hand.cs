using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Models
{
    public class Hand : IComparable<Hand>
    {
        public double Id { get; set; }

        public RankType Rank { get; set; }
        public ICollection<Card> Cards { get; set; }

        public int CompareTo(Hand other)
        {
            if (Rank < other.Rank) return -1;
            if (Rank > other.Rank) return 1;

            return 0;
        }

        public enum RankType
        {
            HighCard, Pair, TwoPair, ThreeOfAKind, Straight, Flush, FullHouse, FourOfAKind, StraightFlush, RoyalFlush
        }
    }
}
