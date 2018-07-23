using hand.history.Models;
using Newtonsoft.Json;
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

        private double Id => Parser.ParseDouble(Text[0], "(?<=Hand #)[0-9]{10,}");
        private double TournamentId => Parser.ParseDouble(Text[0], "(?<=Tournament #)[0-9]{10,}");
        private string Title => Parser.ParseString(Text[1], "'(.*?)'");
        private string Game => Parser.ParseString(Text[0], "(Hold'em No Limit)");
        private string Currency => Parser.ParseString(Text[0], "[$]|[£]|[€]");
        private double Big => Parser.ParseDouble(Text[9], "(?<=big blind [$]|[£]|[€])([+-]?\\d*\\.\\d+)(?![-+0-9\\.])");
        private double Small => Parser.ParseDouble(Text[8], "(?<=small blind [$]|[£]|[€])([+-]?\\d*\\.\\d+)(?![-+0-9\\.])");
        private double TotalPot => Parser.ParseDouble(Text[35], "(?<=Total pot [$]|[£]|[€])([+-]?\\d*\\.\\d+)(?![-+0-9\\.])");
        private double TotalRake => Parser.ParseDouble(Text[35], "(?<=Rake [$]|[£]|[€])([+-]?\\d*\\.\\d+)(?![-+0-9\\.])");
        private int Seats => Parser.ParseInteger(Text[1], "(\\d+)(?=-max)");

        private DateTime Date => Parser.ParseDateTime(Text[0], @"(?<=\[)(.*?)(?=ET\])");

        private ICollection<Player> Players
        {
            get
            {
                var result = new List<Player>();

                var usernamePattern = @"(?<=Seat [0-9]+: )(.+)(?=\()";
                var stackPattern = "";

                for (int i = 2; i < 2 + Seats; i++)
                {
                    var current = Text[i];
                    var username = Parser.ParseString(current, usernamePattern);
                    var stack = Parser.ParseDouble(current, stackPattern);

                    Console.WriteLine("player " + username);

                    result.Add(new Player { Username = username, Stack = stack, Alive = true, Position = i});
                }


                return result;
            }
        }

        private ICollection<Round> Rounds
        {
            get
            {
                var result = new List<Round>();

                return result;
            }
        }

        public Table Map()
        {
            Console.WriteLine("MapId " + Id);
            Console.WriteLine("MapCurrency " + Currency);
            Console.WriteLine("MapDate " + Date);
            Console.WriteLine("MapTitle " + Title);
            Console.WriteLine("BB " + Big);
            Console.WriteLine("SB " + Small);
            Console.WriteLine("MapTotalPot " + TotalPot);
            Console.WriteLine("MapTotalRake " + TotalRake);
            Console.WriteLine("MapSeats " + Seats);

            Console.WriteLine("Players " + Players);


            return new Table();
        }
    }
}
