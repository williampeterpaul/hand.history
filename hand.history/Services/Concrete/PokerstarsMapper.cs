using hand.history.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Services.Concrete
{
    public class PokerstarsMapper : IMapper<Table>
    {
        private IParser Parser { get; }

        private string[] Text { get; }

        public PokerstarsMapper(IParser parser, string text)
        {
            Parser = parser;
            Text = text.Split(Environment.NewLine);
        }

        private double MapId => Parser.ParseDouble(Text[0], "(?<=Hand #)[0-9]{10,}");

        private double MapTournamentId => Parser.ParseDouble(Text[0], "(?<=Tournament #)[0-9]{10,}");

        private string MapTitle => Parser.ParseString(Text[1], "'([^']*)'");

        private string MapGame => Parser.ParseString(Text[0], "(Hold'em No Limit)");

        private string MapCurrency => Parser.ParseString(Text[0], "[$]|[£]|[€]");

        private double MapBig => Parser.ParseDouble(Text[0], "");

        private double MapSmall => Parser.ParseDouble(Text[0], "");

        private double MapTotalPot => Parser.ParseDouble(Text[0], "");

        private double MapTotalRake => Parser.ParseDouble(Text[0], "");

        private int MapSeats => Parser.ParseInteger(Text[0], "");

        //private DateTime MapDate
        //{
        //    get
        //    {
        //        Parser.ParseString(Text, "");
        //    }
        //}

        //private ICollection<Player> MapPlayers
        //{
        //    get
        //    {
        //        Parser.ParseString(Text, "");
        //    }
        //}

        //private ICollection<Round> MapRounds
        //{
        //    get
        //    {
        //        Parser.ParseString(Text, "");
        //    }
        //}

        public Table Map()
        {
            return new Table
            {
                Id = MapId,
                Title = MapTitle,
                Game = MapGame,
                Currency = MapCurrency,
                //Big = MapBig,
                //Small = MapSmall,
                //TotalPot = MapTotalPot,
                //TotalRake = MapTotalRake
            };
        }
    }
}
