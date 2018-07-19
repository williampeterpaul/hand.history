using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Models
{
    public class Action
    {
        public Player Player { get; set; }
        public VerbType Verb { get; set; }
        public decimal Amount { get; set; }

        public enum VerbType
        {
            Fold, Check, Call, Bet, Raise
        }
    }
}
