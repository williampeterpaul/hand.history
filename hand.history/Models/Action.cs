using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Models
{
    public class Action
    {
        public double Id { get; set; }

        public double Amount { get; set; }

        public Player Player { get; set; }
        public VerbType Verb { get; set; }

        public enum VerbType
        {
            Fold, Check, Call, Bet, Raise, Show, Muck
        }
    }
}
