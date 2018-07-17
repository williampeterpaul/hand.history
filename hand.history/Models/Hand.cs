using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Models
{
    public class Hand : IComparable<Hand>
    {
        public ICollection<Card> Cards { get; set; }

        public int CompareTo(Hand other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(Hand other, ICollection<Card> community)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(Hand other, Round round) => CompareTo(other, round.Community);
    }
}
