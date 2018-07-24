using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Models
{
    public class Player
    {
        public double Id { get; set; }

        public string Username { get; set; }
        public bool Alive { get; set; }
        public int Position { get; set; }
        public double Stack { get; set; }

        public Hand Hand { get; set; }
    }
}
