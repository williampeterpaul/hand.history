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
    public sealed class PokerstarsMapperService : IMapper<Table>
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
        private const string SeatIdentifierRegex = @"(Seat [0-9]+\:)";

        private const string StreetVerbRegex = @"(?<=:\s+)\w+";
        private const string DealtToRegex = @"(?<=Dealt to ).*(?= \[)";

        private decimal _pot;
        private decimal _sblind;
        private decimal _bblind;
        private decimal _stackDelta;
        private int _seatsActual;

        private int[] _streetIndexes;
        private IEnumerable<Player> _players;
        private IEnumerable<Street> _streets;

        private IParser Parser { get; }

        public PokerstarsMapperService(IParser parser)
        {
            Parser = parser;
        }

        private IEnumerable<Player> GetPlayers(string[] text)
        {
            var result = new List<Player>();

            for (int i = 0; i < _seatsActual; i++)
            {
                var currentPlayerText = text[i + 2];
                var username = Parser.ParseString(currentPlayerText, AheadColonRegex + AnyCharRegex + BehindBracketRegex).Trim();

                var startingStack = Parser.ParseDecimal(currentPlayerText, AheadBracketRegex + CurrencyRegex);
                var instanceStack = startingStack;

                if (i == 1) instanceStack -= _sblind;
                if (i == 2) instanceStack -= _bblind;

                result.Add(new Player { Username = username, StartingStack = startingStack, InstanceStack = instanceStack, Alive = true, Position = i });
            }

            return result;
        }

        private IEnumerable<Street> GetStreets(string[] text)
        {
            var result = new List<Street>();

            for (int i = 0; i < _streetIndexes.Count() - 1; i++) // exclude summary
            {
                var streetHeading = _streetIndexes[i];
                var streetStart = _streetIndexes[i] + 1;
                var streetEnd = _streetIndexes[i + 1];

                var streetType = (StreetType)i;

                var community = TextToCards(text[streetHeading]);
                var actions = new List<DataObject.Action>();

                for (int j = streetStart; j < streetEnd; j++)
                {
                    var streetLine = text[j];

                    if (i == 0 && j == streetStart) // first line first street
                    {
                        var currentPlayerText = Parser.ParseString(streetLine, DealtToRegex);
                        var currentPlayer = _players.Where(x => x.Username.Equals(currentPlayerText)).Single();

                        currentPlayer.Hand = new Hand { Cards = TextToCards(streetLine) };

                        continue;
                    }

                    actions.Add(TextToAction(streetLine));
                }

                result.Add(new Street { Type = streetType, Community = community, Actions = actions });
            }

            return result;
        }

        private DataObject.Action TextToAction(string text)
        {
            decimal result = 0;

            if (text.Contains("Uncalled")) return null; // not cool
            if (text.Contains("doesn't")) return null; // not cool
            if (text.Contains("collected")) // not cool
            {
                var currentPlayerText = Parser.ParseString(text, AnyCharRegex + BehindCollectedRegex).Trim();
                var currentPlayer = _players.Where(x => x.Username == currentPlayerText).Single();

                result = Parser.ParseDecimal(text, AheadCollectedRegex + CurrencyRegex);

                //currentPlayer.InstanceStack = currentPlayer.StartingStack + result;

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
                result = Parser.ParseDecimal(text, AheadColonRegex + CurrencyRegex);
                player.InstanceStack -= result;
                player.Alive = true;
            }

            if (verb == VerbType.Raises)
            {
                var raise = Parser.ParseDecimalMulti(text, AheadColonRegex + CurrencyRegex);
                result = raise.ElementAt(1) - raise.ElementAt(0);
                player.InstanceStack -= result;
                player.Alive = true;
            }

            return new DataObject.Action { Player = player, Verb = verb, Amount = result };
        }

        public IEnumerable<Card> TextToCards(string text)
        {
            var textAfterSquareBracket = Parser.ParseString(text, AheadSquareBracketRegex + AnyCharRegex);
            var cardsText = Parser.ParseStringMulti(textAfterSquareBracket, AnyCardRegex);

            return cardsText.ConvertType(TextToCard);
        }

        public Card TextToCard(string text)
        {
            if (text.Length != 2) throw new FormatException("Text must be in the format [Rs] : where R is rank and s is suit for example 9h, As, or Tc");

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
            if (text.Length < 10) throw new FormatException("Text array length is less than the minimum possible length of a game");
            if (text.Length > 99) throw new FormatException("Text array length is greater than the maximum possible length of a game");

            _streetIndexes = text.FindIndexes(x => Parser.ParseString(x, StreetIdentifierRegex).Length > 0).ToArray();
            _seatsActual = text.FindIndexes(x => Parser.ParseString(x, SeatIdentifierRegex).Length > 0).Count() / 2;

            if (_streetIndexes.Length < 2) throw new FormatException("Street indexes array length is less than the minimum possible amount of streets");
            if (_streetIndexes.Length > 6) throw new FormatException("Street indexes array length is greater than the maximum possible amount of streets");

            _sblind = Parser.ParseDecimal(text[_seatsActual + 2], AheadBlindRegex + CurrencyRegex);
            _bblind = Parser.ParseDecimal(text[_seatsActual + 3], AheadBlindRegex + CurrencyRegex);

            if (_bblind < _sblind) throw new FormatException("Big blind must be greater than the small blind");

            _players = GetPlayers(text);

            if (_players.Count() != _seatsActual) throw new FormatException("Player count must be equal to the seats occupied");

            _streets = GetStreets(text);

            if (_streets.Count() != _streetIndexes.Length - 1) throw new FormatException("Street count must be equal to the street indexes array length");

            var table = new Table
            {
                HandId = Parser.ParseDecimalMulti(text[0], AheadHashRegex + AnyNumberRegex).ElementAtOrDefault(0),
                TournamentId = Parser.ParseDecimalMulti(text[0], AheadHashRegex + AnyNumberRegex).ElementAtOrDefault(1),
                SBlind = _sblind,
                BBlind = _bblind,
                Pot = Parser.ParseDecimalMulti(text[_streetIndexes.Last() + 1], CurrencyRegex).ElementAtOrDefault(0),
                Rake = Parser.ParseDecimalMulti(text[_streetIndexes.Last() + 1], CurrencyRegex).ElementAtOrDefault(1),
                Currency = Parser.ParseString(text[0], CurrencyUnitRegex),
                Title = Parser.ParseString(text[1], AheadQuoteRegex + AnyCharRegex + BehindQuoteRegex),
                Game = Parser.ParseString(text[0], GameIdentifierRegex),
                SeatsActual = _seatsActual,
                SeatsMax = Parser.ParseInteger(text[1], AnyNumberRegex + BehindMaxRegex),
                Date = Parser.ParseDateTime(text[0], AheadSquareBracketRegex + AnyDateRegex),
                Players = _players,
                Streets = _streets
            };

            if (_seatsActual > table.SeatsMax) throw new FormatException("Seats occupied must be less than or equal to the table max");

            _stackDelta = _players.Sum(x => x.StartingStack - x.InstanceStack);

            if(_stackDelta != table.Pot) throw new FormatException("Winning players' increase must equal the pot");

            // var currentEasternTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            // if (currentEasternTime < table.Date) throw new FormatException("Date of game must be in the past");

            return table;
        }
    }
}
