using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.DataObject
{
    public class Action
    {
        public double Id { get; set; }
        public double Amount { get; set; }

        public Player Player { get; set; }
        public VerbType Verb { get; set; }

        public enum VerbType
        {
            Folds, Checks, Calls, Bets, Raises, Shows, Mucks
        }
    }
}
