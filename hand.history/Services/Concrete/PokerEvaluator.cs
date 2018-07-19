using hand.history.Extensions;
using hand.history.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static hand.history.Models.Hand;

namespace hand.history.Services.Concrete
{
    public sealed class PokerEvaluator : IEvaluator
    {
        private IReadOnlyDictionary<RankType, Func<IEnumerable<Card>, bool>> Classifier { get; } = new Dictionary<RankType, Func<IEnumerable<Card>, bool>>
        {
            { RankType.RoyalFlush,      cards => cards.SequentialValues(5).GroupBy(card => card.Suit).Any(group => group.Count() > 4) && cards.Max(x => x.Rank).Equals(Card.RankType.Ace) },
            { RankType.StraightFlush,   cards => cards.SequentialValues(5).GroupBy(card => card.Suit).Any(group => group.Count() > 4) },
            { RankType.FourOfAKind,     cards => cards.GroupBy(card => card.Rank).Any(group => group.Count() == 4) },
            { RankType.FullHouse,       cards => cards.GroupBy(card => card.Rank).Count(group => group.Count() == 2) > 0 && cards.GroupBy(card => card.Rank).Count(group => group.Count() == 3) > 0 },
            { RankType.Flush,           cards => cards.GroupBy(card => card.Suit).Any(group => group.Count() > 4) },
            { RankType.Straight,        cards => cards.SequentialValues(5).Any() },
            { RankType.ThreeOfAKind,    cards => cards.GroupBy(card => card.Rank).Any(group => group.Count() == 3) },
            { RankType.TwoPair,         cards => cards.GroupBy(card => card.Rank).Count(group => group.Count() == 2) > 1 },
            { RankType.Pair,            cards => cards.GroupBy(card => card.Rank).Any(group => group.Count() == 2) },
            { RankType.HighCard,        cards => true },
        };

        public RankType Evalute(IEnumerable<Card> cards)
        {
            if (cards == null) throw new ArgumentNullException("Card reference cannot be null");
            if (cards.Count() > 7) throw new ArgumentException("Card count must be between 1 and 7");

            return Classifier.Where(x => x.Value(cards)).First().Key;
        }
    }
}
