using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Models
{
    public class Player
    {
        public decimal Id { get; }

        public string Username { get; }
        public bool Alive { get; set; }
        public int Position { get; set; }
        public decimal Stack { get; set; }

        public Hand Hand { get; }
    }
}
