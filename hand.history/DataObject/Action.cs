using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.DataObject
{
    public class Action
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }

        public Player Player { get; set; }
        public VerbType Verb { get; set; }

        public enum VerbType
        {
            Folds, Checks, Calls, Bets, Raises, Shows, Mucks
        }

        public override string ToString() => $"{Player} - {Verb}";
    }
}
