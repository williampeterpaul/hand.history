using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.DataObject
{
    public class Player
    {
        public double Id { get; set; }
        public double Stack { get; set; }
        public int Position { get; set; }
        public bool Alive { get; set; }
        public string Username { get; set; }

        public Hand Hand { get; set; }
    }
}
