﻿using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.DataObject
{
    public class Round
    {
        public double Id { get; set; }

        public double Pot { get; set; }

        public StreetType Street { get; set; }
        public ICollection<Card> Community { get; set; }
        public ICollection<Action> Actions { get; set; }

        public enum StreetType
        {
            Preflop, Flop, Turn, River, Showdown
        }
    }
}