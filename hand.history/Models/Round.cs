using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Models
{
    public class Round
    {
        public decimal Pot { get; set; }

        public ICollection<Card> Community { get; }

        public ICollection<Player> ActivePlayers { get; }
        public ICollection<Player> DeadPlayers { get; }
    }
}
