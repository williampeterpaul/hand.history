using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.DataObject
{
    public class Player
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public decimal StartingStack { get; set; }
        public decimal EndingStack { get; set; }
        public string Username { get; set; }
        public bool Alive { get; set; }

        public Hand Hand { get; set; }

        public override string ToString() => $"{Username} {EndingStack - StartingStack}";
    }
}
