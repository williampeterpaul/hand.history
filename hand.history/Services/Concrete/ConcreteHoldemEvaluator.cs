using hand.history.Extensions;
using hand.history.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static hand.history.Models.Hand;

namespace hand.history.Services.Concrete
{
    public sealed class ConcreteHoldemEvaluator : IEvaluator<IEnumerable<Card>>
    {
        public IReadOnlyDictionary<RankType, Func<IEnumerable<Card>, bool>> Classifier = new Dictionary<RankType, Func<IEnumerable<Card>, bool>>
        {
            { RankType.RoyalFlush, IsRoyalFlush },
            { RankType.StraightFlush, IsStraightFlush },
            { RankType.FourOfAKind, IsFourOfAKind },
            { RankType.FullHouse, IsFullHouse },
            { RankType.Flush, IsFlush },
            { RankType.Straight, IsStraight },
            { RankType.ThreeOfAKind, IsThreeOfAKind },
            { RankType.TwoPair, IsTwoPair },
            { RankType.Pair, IsPair },
            { RankType.HighCard, IsHighCard },
        };

        public static bool IsRoyalFlush(IEnumerable<Card> cards)
        {
            return cards.SequentialValues(5).GroupBy(card => card.Suit).Any(group => group.Count() > 4)
                && cards.Max(x => x.Rank).Equals(Card.RankType.Ace);
        }

        public static bool IsStraightFlush(IEnumerable<Card> cards)
        {
            return cards.SequentialValues(5).GroupBy(card => card.Suit).Any(group => group.Count() > 4);
        }

        public static bool IsFourOfAKind(IEnumerable<Card> cards)
        {
            return cards.GroupBy(card => card.Rank).Any(group => group.Count() == 4);
        }

        public static bool IsFullHouse(IEnumerable<Card> cards)
        {
            return cards.GroupBy(card => card.Rank).Count(group => group.Count() == 2) > 0 
                && cards.GroupBy(card => card.Rank).Count(group => group.Count() == 3) > 0;
        }

        public static bool IsFlush(IEnumerable<Card> cards)
        {
            return cards.GroupBy(card => card.Suit).Any(group => group.Count() > 4);
        }

        public static bool IsStraight(IEnumerable<Card> cards)
        {
            return cards.SequentialValues(5).Any();
        }

        public static bool IsThreeOfAKind(IEnumerable<Card> cards)
        {
            return cards.GroupBy(card => card.Rank).Any(group => group.Count() == 3);
        }

        public static bool IsTwoPair(IEnumerable<Card> cards)
        {
            return cards.GroupBy(card => card.Rank).Count(group => group.Count() == 2) > 1;
        }

        public static bool IsPair(IEnumerable<Card> cards)
        {
            return cards.GroupBy(card => card.Rank).Any(group => group.Count() == 2);
        }

        public static bool IsHighCard(IEnumerable<Card> cards)
        {
            return true;
        }

        public RankType Evalute(IEnumerable<Card> cards)
        {
            if (cards == null) throw new ArgumentNullException("Card reference cannot be null");
            if (cards.Count() > 7) throw new ArgumentException("Card count must be between 1 and 7");

            return Classifier.Where(x => x.Value(cards)).First().Key;
        }
    }
}
