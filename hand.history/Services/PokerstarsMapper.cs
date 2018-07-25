using hand.history.Extensions;
using hand.history.DataObject;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static hand.history.DataObject.Action;
using static hand.history.DataObject.Round;
using hand.history.Services.Interfaces;

namespace hand.history.Services
{
    public class PokerstarsMapper : IMapper<Table>
    {
        private IParser Parser { get; }

        private string[] Text { get; set; }

        public PokerstarsMapper(IParser parser)
        {
            Parser = parser;
        }

        private double Id => Parser.ParseDouble(Text[0], "(?<=Hand #)[0-9]{10,}");

        private double TournamentId => Parser.ParseDouble(Text[0], "(?<=Tournament #)[0-9]{10,}");

        private string Title => Parser.ParseString(Text[1], "'(.*?)'");

        private string Game => Parser.ParseString(Text[0], "(Hold'em No Limit)");

        private string Currency => Parser.ParseString(Text[0], "[$]|[£]|[€]");

        private double Big => Parser.ParseDouble(Text[9], "(?<=blind [$]|[£]|[€])([+-]?\\d*\\.\\d+)(?![-+0-9\\.])");

        private double Small => Parser.ParseDouble(Text[8], "(?<=blind [$]|[£]|[€])([+-]?\\d*\\.\\d+)(?![-+0-9\\.])");

        private double TotalPot => Parser.ParseDouble(Text[35], "(?<=Total pot [$]|[£]|[€])([+-]?\\d*\\.\\d+)(?![-+0-9\\.])");

        private double TotalRake => Parser.ParseDouble(Text[35], "(?<=Rake [$]|[£]|[€])([+-]?\\d*\\.\\d+)(?![-+0-9\\.])");

        private int Seats => Parser.ParseInteger(Text[1], "(\\d+)(?=-max)");

        private DateTime Date => Parser.ParseDateTime(Text[0], "(?<=\\[)(.*?)(?=ET\\])");

        private ICollection<Player> Players
        {
            get
            {
                var result = new List<Player>();

                var usernamePattern = @"(?<=Seat [0-9]+: )(.+)(?=\()";
                var stackPattern = @"(?<=\(([$]|[£]|[€]))(.+)(?=in)";

                for (int i = 0; i < Seats; i++)
                {
                    var current = Text[i + 2];
                    var username = Parser.ParseString(current, usernamePattern);
                    var stack = Parser.ParseDouble(current, stackPattern);

                    result.Add(new Player { Username = username, Stack = stack, Alive = true, Position = i });
                }


                return result;
            }
        }

        private ICollection<Round> Rounds
        {
            get
            {
                var result = new List<Round>();

                var communityPattern = "(\\[).*(?=[\\n|\\r|\\s])";
                var streetsPattern = "(\\*\\*\\*) (\\w+).*[\\n|\\r|\\s]*";
                var usernamePattern = ".*(?=:)";
                var verbPattern = "(?<=:\\s+)\\w+";
                var amountPattern = "([$]|[£]|[€]).*[\\n|\\r|\\s]*";

                var streetIndexes = Text.FindIndexes(x => Parser.ParseString(x, streetsPattern) != default(string)).ToArray();

                // don't care about the summary
                for (int i = 0; i < streetIndexes.Count() - 1; i++)
                {
                    var roundStart = streetIndexes[i];
                    var roundEnd = streetIndexes[i + 1];

                    var round = (StreetType)i;
                    var community = new List<Card>();

                    var cards = Parser.ParseString(Text[roundStart], communityPattern);

                    var actions = new List<DataObject.Action>();

                    // skip first line; street title
                    for (int j = roundStart + 1; j < roundEnd; j++)
                    {
                        // first street, first line always prints user dealt hole cards
                        if (i == 0 && j == roundStart + 1) continue;

                        var roundLine = Text[j];

                        var playername = Parser.ParseString(roundLine, usernamePattern);
                        var verbname = Parser.ParseString(roundLine, verbPattern);
                        //var amount = Parser.ParseDouble(roundLine, amountPattern);

                        //var player = Players.Where(x => x.Username == playername).FirstOrDefault();
                        //var verb = verbname.TrimEnd('s').ToEnum<VerbType>();

                        //if (verb == VerbType.Fold)
                        //{
                        //    player.Alive = false;
                        //}

                        //if (verb == VerbType.Check || verb == VerbType.Fold)
                        //{
                        //    amount = 0;
                        //}

                        //var act = new Models.Action
                        //{
                        //    Player = player,
                        //    Verb = verb,
                        //    Amount = amount
                        //};

                        //actions.Add(act);
                    }


                    result.Add(new Round { Street = round, Community = community, Actions = actions });
                }

                return result;
            }
        }

        private Card TextToCard(string text)
        {
            if (text.Length != 2) throw new FormatException("Text is not in the format correct format");

            return new Card { Rank = Card.RankType.Ace, Suit = Card.SuitType.Club };
        }

        public Table Map(string[] text)
        {
            Text = text;

            Console.WriteLine("MapId " + Id);
            Console.WriteLine("MapCurrency " + Currency);
            Console.WriteLine("MapDate " + Date);
            Console.WriteLine("MapTitle " + Title);
            Console.WriteLine("BB " + Big);
            Console.WriteLine("SB " + Small);
            Console.WriteLine("MapTotalPot " + TotalPot);
            Console.WriteLine("MapTotalRake " + TotalRake);
            Console.WriteLine("MapSeats " + Seats);

            var k = Rounds;

            return new Table();
        }
    }
}
