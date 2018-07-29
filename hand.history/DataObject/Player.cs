using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.DataObject
{
    public class Player
    {
        public decimal Id { get; set; }
        public decimal Stack { get; set; }
        public string Username { get; set; }
        public int Position { get; set; }
        public bool Alive { get; set; }

        public Hand Hand { get; set; }
    }
}
