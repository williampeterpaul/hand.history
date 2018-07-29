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
using static hand.history.DataObject.Card;

namespace hand.history.Services
{
    public class PokerstarsMapperService : IMapper<Table>
    {
        private const string CurrencyRegex =            @"(\d{1,3}(,?\d{3})?(\.\d\d?)?|\(\$?\d{1,3}(,?\d{3})?(\.\d\d?)?\))";
        private const string CurrencyUnitRegex =        @"([$]|[£]|[€])";
        private const string AnyCharRegex =             @"(.*)";
        private const string AnyCardRegex =             @"([AKQJT]|[0-9])([cdhs])";
        private const string BehindColonRegex =         @"(?=\:.*)";
        private const string BehindBracketRegex =       @"(?=\(.*)";
        private const string AheadColonRegex =          @"(?<=\:.*)";
        private const string AheadBracketRegex =        @"(?<=\(.*)";
        private const string AheadSquareBracketRegex =  @"(?<=\[.*)";
        private const string AheadBlindRegex =          @"(?<=blind.*)";
        private const string StreetIdentifierPattern =  @"(\*\*\*).*";

        private const string StreetVerbPattern =        @"(?<=:\s+)\w+";
        private const string DealtToPattern =           @"(?<=Dealt to ).*(?= \[)";

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
        private IEnumerable<Street> _streets;

        private string[] Text { get; set; }

        private IParser Parser { get; }

        public PokerstarsMapperService(IParser parser)
        {
            Parser = parser;
        }

        private double Id => Parser.ParseDouble(Text[0], "(?<=Hand #)[0-9]{10,}");

        private double TournamentId => Parser.ParseDouble(Text[0], "(?<=Tournament #)[0-9]{10,}");

        private string Title => Parser.ParseString(Text[1], "'(.*?)'");

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
                    var username = Parser.ParseString(current, AheadColonRegex + AnyCharRegex + BehindBracketRegex).Trim();
                    var stack = Parser.ParseDouble(current, AheadBracketRegex + CurrencyRegex);

                    result.Add(new Player { Username = username, Stack = stack, Alive = true, Position = i });
                }

                return result;
            }
        }

        private IEnumerable<Street> Streets
        {
            get
            {
                var result = new List<Street>();
                var streetIndexes = Text.FindIndexes(x => Parser.ParseString(x, StreetIdentifierPattern) != string.Empty).ToArray();

                for (int i = 0; i < streetIndexes.Count() - 1; i++)
                {
                    var streetStart = streetIndexes[i];
                    var streetEnd = streetIndexes[i + 1];

                    var streetType = (StreetType)i;

                    var community = TextToCards(Text[streetStart]);
                    var actions = new List<DataObject.Action>();

                    for (int j = streetStart + 1; j < streetEnd; j++)
                    {
                        var streetLine = Text[j];
                        double amount = 0;

                        if (i == 0 && j == streetStart + 1)
                        {
                            var currentPlayerText = Parser.ParseString(streetLine, DealtToPattern);
                            var currentPlayer = Players.Where(x => x.Username.Equals(currentPlayerText)).Single();

                            currentPlayer.Hand = new Hand() { Cards = TextToCards(streetLine) };

                            continue;
                        }

                        if (streetLine.Contains("collected"))
                        {
                            // get player
                            // get amount and add to stack
                            // if amount != pot - rake then throw
                            // set alive to true!
                            break;
                        }

                        var verbText = Parser.ParseString(streetLine, StreetVerbPattern);
                        var playerText = Parser.ParseString(streetLine, AnyCharRegex + BehindColonRegex);

                        var verb = verbText.ToEnum<VerbType>();
                        var player = Players.Where(x => x.Username.Equals(playerText)).Single();

                        if (verb == VerbType.Folds) player.Alive = false;
                        if (verb == VerbType.Mucks) player.Alive = false;
                        if (verb == VerbType.Shows) player.Alive = false; player.Hand = new Hand { Cards = TextToCards(streetLine) };
                        if (verb == VerbType.Bets || verb == VerbType.Calls)
                        {
                            amount = Parser.ParseDouble(streetLine, AheadColonRegex + CurrencyRegex);
                            player.Stack -= amount;
                        }
                        if (verb == VerbType.Raises)
                        {
                            var raise = Parser.ParseDoubleMulti(streetLine, AheadColonRegex + CurrencyRegex);
                            amount = raise.ElementAt(1) - raise.ElementAt(0);
                            player.Stack -= amount;
                        }

                        actions.Add(new DataObject.Action { Player = player, Verb = verb, Amount = amount });
                    }

                    result.Add(new Street { Type = streetType, Community = community, Actions = actions });
                }

                return result;
            }
        }

        public IEnumerable<Card> TextToCards(string text)
        {
            var textAfterSquareBracket = Parser.ParseString(text, AheadSquareBracketRegex + AnyCharRegex);
            var cardsText = Parser.ParseStringMulti(textAfterSquareBracket, AnyCardRegex);

            return cardsText.ConvertType(TextToCard);
        }

        public Card TextToCard(string text)
        {
            if (text.Length != 2) throw new FormatException("Text is not in the format correct format! Examples include 9h, As, and Tc");

            var result = new Card();

            var rank = text[0];
            var suit = text[1];

            switch (rank)
            {
                case 'A': result.Rank = RankType.Ace; break;
                case 'K': result.Rank = RankType.King; break;
                case 'Q': result.Rank = RankType.Queen; break;
                case 'J': result.Rank = RankType.Jack; break;
                case 'T': result.Rank = RankType.Ten; break;
                case '9': result.Rank = RankType.Nine; break;
                case '8': result.Rank = RankType.Eight; break;
                case '7': result.Rank = RankType.Seven; break;
                case '6': result.Rank = RankType.Six; break;
                case '5': result.Rank = RankType.Five; break;
                case '4': result.Rank = RankType.Four; break;
                case '3': result.Rank = RankType.Three; break;
                case '2': result.Rank = RankType.Two; break;

                default: throw new FormatException("Rank cannot be parsed! Only A, K, Q, J, T, or 2 to 9 allowed");
            }

            switch (suit)
            {
                case 'c': result.Suit = SuitType.Club; break;
                case 's': result.Suit = SuitType.Spade; break;
                case 'h': result.Suit = SuitType.Heart; break;
                case 'd': result.Suit = SuitType.Diamond; break;

                default: throw new FormatException("Suit cannot be parsed! Only c, s, h, or d allowed");
            }

            return result;
        }

        public Table Map(string[] text)
        {
            Text = text;

            var k = Streets;

            return new Table();
        }
    }
}
