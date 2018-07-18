using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Models
{
    public class Player
    {
        public string Username { get; }
        public decimal Stack { get; set; }
        public int Position { get; set; }
        public bool Alive { get; set; }

        public Hand Hand { get; }
    }
}
