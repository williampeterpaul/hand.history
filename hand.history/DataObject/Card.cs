using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.DataObject
{
    public class Card : IComparable<Card>
    {
        public int Id { get; set; }

        public RankType Rank { get; set; }
        public SuitType Suit { get; set; }

        public int CompareTo(Card other)
        {
            if (Rank < other.Rank) return -1;
            if (Rank > other.Rank) return 1;

            return 0;
        }

        public enum RankType
        {
            Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace
        }

        public enum SuitType
        {
            Club, Diamond, Heart, Spade
        }
    }
}
