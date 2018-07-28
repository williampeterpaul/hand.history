using hand.history.Extensions;
using hand.history.DataObject;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static hand.history.DataObject.Action;
using static hand.history.DataObject.Street;
using hand.history.Services.Interfaces;

namespace hand.history.Services
{
    public class PokerstarsMapperService : IMapper<Table>
    {
        private const string CurrencyRegex = @"(\d{1,3}(,?\d{3})?(\.\d\d?)?|\(\$?\d{1,3}(,?\d{3})?(\.\d\d?)?\))";
        private const string CurrencyUnitRegex = @"([$]|[£]|[€])";

        private const string AfterColonRegex = @"(?<=:.*)";
        private const string AfterBracketRegex = @"(?<=(.*)";
        private const string AfterBracketAltRegex = @"(?<=[.*)";
        private const string AfterBlindRegex = @"(?<=blind)";

        private const string RoundVerbPattern = @"(?<=:\s+)\w+";
        private const string RoundUsernamePattern = @".*(?=:)";
        private const string RoundStreetsPattern = @"(\*\*\*) (\w+).*[\n|\r|\s]*";
        private const string RoundCommunityPattern = @"(\[).*(?=[\\n|\r|\s])";

        private const string PlayersUsernamePattern = @"(?<=Seat [0-9]+: )(.+)(?= \()";
        private const string PlayersStackPattern = @"(?<=\(([$]|[£]|[€]))(.+)(?=in)";

        private string _id;
        private string _tourneyId;
        private string _title;
        private string _game;
        private string _currency;
        private string _bblind;
        private string _sblind;
        private string _pot;
        private string _rake;
        private string _seats;

        private DateTime _date;
        private IEnumerable<Player> _players;
        private IEnumerable<Street> _rounds;

        private string[] Text { get; set; }

        private IParser Parser { get; }

        public PokerstarsMapperService(IParser parser)
        {
            Parser = parser;
        }

        private double Id => Parser.ParseDouble(Text[0], "(?<=Hand #)[0-9]{10,}");

        private double TournamentId => Parser.ParseDouble(Text[0], "(?<=Tournament #)[0-9]{10,}");

        private string Title=> Parser.ParseString(Text[1], "'(.*?)'");

        private string Game => Parser.ParseString(Text[0], "(Hold'em No Limit)");

        private string Currency => Parser.ParseString(Text[0], "[$]|[£]|[€]");

        private double BigBlind => Parser.ParseDouble(Text[9], "(?<=blind [$]|[£]|[€])([+-]?\\d*\\.\\d+)(?![-+0-9\\.])");

        private double SmallBlind => Parser.ParseDouble(Text[8], "(?<=blind [$]|[£]|[€])([+-]?\\d*\\.\\d+)(?![-+0-9\\.])");

        private double TotalPot => Parser.ParseDouble(Text[35], "(?<=Total pot [$]|[£]|[€])([+-]?\\d*\\.\\d+)(?![-+0-9\\.])");

        private double TotalRake => Parser.ParseDouble(Text[35], "(?<=Rake [$]|[£]|[€])([+-]?\\d*\\.\\d+)(?![-+0-9\\.])");

        private int Seats => Parser.ParseInteger(Text[1], "(\\d+)(?=-max)");

        private DateTime Date => Parser.ParseDateTime(Text[0], "(?<=\\[)(.*?)(?=ET\\])");

        private IEnumerable<Player> Players
        {
            get
            {
                var result = new List<Player>();

                for (int i = 0; i < Seats; i++)
                {
                    var current = Text[i + 2];
                    var username = Parser.ParseString(current, PlayersUsernamePattern);
                    var stack = Parser.ParseDouble(current, PlayersStackPattern);

                    result.Add(new Player { Username = username, Stack = stack, Alive = true, Position = i });
                }

                return result;
            }
        }

        private IEnumerable<Street> Rounds
        {
            get
            {
                var result = new List<Street>();
                var streetIndexes = Text.FindIndexes(x => Parser.ParseString(x, RoundStreetsPattern) != default(string)).ToArray();

                // don't care about the summary so take 1
                for (int i = 0; i < streetIndexes.Count() - 1; i++)
                {
                    var streetStart = streetIndexes[i];
                    var streetEnd = streetIndexes[i + 1];

                    var round = (StreetType)i;
                    var community = new List<Card>();

                    var cards = Parser.ParseString(Text[streetStart], RoundCommunityPattern); //todo parse community cards properly

                    var actions = new List<DataObject.Action>();

                    // skip first line; street title
                    for (int j = streetStart + 1; j < streetEnd; j++)
                    {
                        var streetLine = Text[j];
                        double amount = 0;

                        // first street, first line always prints user dealt hole cards
                        if (i == 0 && j == streetStart + 1)
                        {
                            // get initial cards for player
                            continue;
                        }

                        //todo last line get collected amount and compare to expected, throw exception if fail
                        if (streetLine.Contains("collected"))
                        {
                            // get amount and add to stack
                            // set alive to true
                            continue;
                        }

                        var playerText = Parser.ParseString(streetLine, RoundUsernamePattern);
                        var verbText = Parser.ParseString(streetLine, RoundVerbPattern);

                        var player = Players.Where(x => x.Username.Equals(playerText)).First();
                        var verb = verbText.ToEnum<VerbType>();


                        // skip doesn't show

                        if (verb == VerbType.Bets || verb == VerbType.Calls) amount = Parser.ParseDouble(streetLine, AfterColonRegex + CurrencyRegex);
                        if (verb == VerbType.Folds || verb == VerbType.Mucks) player.Alive = false;
                        if (verb == VerbType.Shows) player.Alive = false; //todo get cards shown
                        if (verb == VerbType.Raises)
                        {
                            var raise = Parser.ParseDoubleMulti(streetLine, AfterColonRegex + CurrencyRegex);
                            amount = raise.ElementAt(1) - raise.ElementAt(0);
                        }

                        actions.Add(new DataObject.Action { Player = player, Verb = verb, Amount = amount });
                    }

                    result.Add(new Street { StreetType = round, Community = community, Actions = actions });
                }

                return result;
            }
        }

        public Card TextToCard(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Text is null or whitespace");
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
            Console.WriteLine("BB " + BigBlind);
            Console.WriteLine("SB " + SmallBlind);
            Console.WriteLine("MapTotalPot " + TotalPot);
            Console.WriteLine("MapTotalRake " + TotalRake);
            Console.WriteLine("MapSeats " + Seats);

            var p = Players;
            var k = Rounds;

            return new Table();
        }
    }
}
