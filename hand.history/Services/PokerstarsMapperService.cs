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
        private const string CurrencyRegex = @"(\d{1,3}(,?\d{3})?(\.\d\d?)?|\(\$?\d{1,3}(,?\d{3})?(\.\d\d?)?\))";
        private const string CurrencyUnitRegex = @"([$]|[£]|[€])";

        private const string AnyCharRegex = @"(.*)";
        private const string AnyNumberRegex = @"([0-9]+)";
        private const string AnyCardRegex = @"([AKQJT]|[0-9])([cdhs])";
        private const string AnyDateRegex = @"([0-9]{4}\/[0-9]{2}\/[0-9]{2} [0-9]{1,2}\:[0-9]{1,2}\:[0-9]{1,2})";

        private const string AheadColonRegex = @"(?<=\:.*)";
        private const string AheadBracketRegex = @"(?<=\(.*)";
        private const string AheadSquareBracketRegex = @"(?<=\[.*)";
        private const string AheadBlindRegex = @"(?<=blind.*)";
        private const string AheadCollectedRegex = @"(?<=collected.*)";
        private const string AheadHashRegex = @"(?<=#.*)";
        private const string AheadQuoteRegex = @"(?<=\')";

        private const string BehindColonRegex = @"(?=\:.*)";
        private const string BehindBracketRegex = @"(?=\(.*)";
        private const string BehindCollectedRegex = @"(?=collected)";
        private const string BehindMaxRegex = @"(?=-max)";
        private const string BehindQuoteRegex = @"(?=\')";

        private const string StreetIdentifierRegex = @"(\*\*\*).*";
        private const string GameIdentifierRegex = @"(Hold'em No Limit)";

        private const string StreetVerbRegex = @"(?<=:\s+)\w+";
        private const string DealtToRegex = @"(?<=Dealt to ).*(?= \[)";

        private decimal _handId;
        private decimal _tourneyId;
        private decimal _bblind;
        private decimal _sblind;
        private decimal _pot;
        private decimal _rake;
        private string _currency;
        private string _title;
        private string _game;
        private int _seats;

        private DateTime _date;
        private IEnumerable<Player> _players;
        private IEnumerable<Street> _streets;
        private int[] _streetIndexes;

        private string[] Text { get; set; }

        private IParser Parser { get; }

        public PokerstarsMapperService(IParser parser)
        {
            Parser = parser;
        }

        private IEnumerable<Player> Players
        {
            get
            {
                var result = new List<Player>();

                for (int i = 0; i < _seats; i++)
                {
                    var current = Text[i + 2];
                    var username = Parser.ParseString(current, AheadColonRegex + AnyCharRegex + BehindBracketRegex).Trim();
                    var stack = Parser.ParseDecimal(current, AheadBracketRegex + CurrencyRegex);

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

                for (int i = 0; i < _streetIndexes.Count() - 1; i++)
                {
                    var streetStart = _streetIndexes[i];
                    var streetEnd = _streetIndexes[i + 1];

                    var streetType = (StreetType)i;

                    var community = TextToCards(Text[streetStart]);
                    var actions = new List<DataObject.Action>();

                    for (int j = streetStart + 1; j < streetEnd; j++)
                    {
                        var streetLine = Text[j];

                        if (i == 0 && j == streetStart + 1)
                        {
                            var currentPlayerText = Parser.ParseString(streetLine, DealtToRegex);
                            var currentPlayer = _players.Where(x => x.Username.Equals(currentPlayerText)).Single();

                            currentPlayer.Hand = new Hand() { Cards = TextToCards(streetLine) };

                            continue;
                        }

                        actions.Add(TextToAction(streetLine));
                    }

                    result.Add(new Street { Type = streetType, Community = community, Actions = actions });
                }

                return result;
            }
        }

        private DataObject.Action TextToAction(string text)
        {
            decimal amount = 0;

            if (text.Contains("collected"))
            {
                var player123 = Parser.ParseString(text, AnyCharRegex + BehindCollectedRegex).Trim();
                var player1 = _players.Where(x => x.Username == player123).Single();

                amount = Parser.ParseDecimal(text, AheadCollectedRegex + CurrencyRegex);

                if (!amount.Equals(_pot - _rake)) throw new FormatException("Collected amount doesn't equal the total pot minus the rake");

                player1.Stack += amount;

                return default(DataObject.Action);
            }

            var verbText = Parser.ParseString(text, StreetVerbRegex);
            var playerText = Parser.ParseString(text, AnyCharRegex + BehindColonRegex);

            var verb = verbText.ToEnum<VerbType>();
            var player = _players.Where(x => x.Username.Equals(playerText)).Single();

            player.Alive = false;

            if (verb == VerbType.Shows)
            {
                player.Hand = new Hand { Cards = TextToCards(text) };
            }

            if (verb == VerbType.Checks)
            {
                player.Alive = true;
            }

            if (verb == VerbType.Bets || verb == VerbType.Calls)
            {
                amount = Parser.ParseDecimal(text, AheadColonRegex + CurrencyRegex);
                player.Stack -= amount;
                player.Alive = true;
            }

            if (verb == VerbType.Raises)
            {
                var raise = Parser.ParseDecimalMulti(text, AheadColonRegex + CurrencyRegex);
                amount = raise.ElementAt(1) - raise.ElementAt(0);
                player.Stack -= amount;
                player.Alive = true;
            }

            return new DataObject.Action { Player = player, Verb = verb, Amount = amount };
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

            _streetIndexes = Text.FindIndexes(x => Parser.ParseString(x, StreetIdentifierRegex).Length > 0).ToArray();

            _handId = Parser.ParseDecimalMulti(Text[0], AheadHashRegex + AnyNumberRegex).ElementAtOrDefault(0);
            _tourneyId = Parser.ParseDecimalMulti(Text[0], AheadHashRegex + AnyNumberRegex).ElementAtOrDefault(1);
            _currency = Parser.ParseString(Text[0], CurrencyUnitRegex);

            _seats = Parser.ParseInteger(Text[1], AnyNumberRegex + BehindMaxRegex);
            _title = Parser.ParseString(Text[1], AheadQuoteRegex + AnyCharRegex + BehindQuoteRegex);
            _game = Parser.ParseString(Text[0], GameIdentifierRegex);
            _sblind = Parser.ParseDecimal(Text[_seats + 2], AheadBlindRegex + CurrencyRegex);
            _bblind = Parser.ParseDecimal(Text[_seats + 3], AheadBlindRegex + CurrencyRegex);
            _pot = Parser.ParseDecimalMulti(Text[_streetIndexes.Last() + 1], CurrencyRegex).ElementAtOrDefault(0);
            _rake = Parser.ParseDecimalMulti(Text[_streetIndexes.Last() + 1], CurrencyRegex).ElementAtOrDefault(1);

            _date = Parser.ParseDateTime(Text[0], AheadSquareBracketRegex + AnyDateRegex);
            _players = Players;
            _streets = Streets;

            var t = new Table
            {
                HandId = _handId,
                TournamentId = _tourneyId,
                BBlind = _bblind,
                SBlind = _sblind,
                Pot = _pot,
                Rake = _rake,
                Currency = _currency,
                Title = _title,
                Game = _game,
                Seats = _seats,
                Date = _date,
                Players = _players,
                Streets = _streets
            };

            return t;
        }
    }
}
