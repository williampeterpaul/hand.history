using hand.history.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static hand.history.Models.Hand;

namespace hand.history.Services.Concrete
{
    public class ConcreteHoldemEvaluator : IEvaluator<IEnumerable<Card>>
    {
        public IReadOnlyDictionary<RankType, Func<IEnumerable<Card>, bool>> Classifier = new Dictionary<RankType, Func<IEnumerable<Card>, bool>>
        {
            { RankType.RoyalFlush, cards => cards.GroupBy(card => card.Suit).Count() == 1 },
            { RankType.StraightFlush, cards => cards.GroupBy(card => card.Suit).Count() == 1 },
            { RankType.FourOfAKind, cards => cards.GroupBy(card => card.Suit).Count() == 1 },
            { RankType.FullHouse, cards => cards.GroupBy(card => card.Suit).Count() == 1 },
            { RankType.Flush, cards => cards.GroupBy(card => card.Suit).Count() == 1 },
            { RankType.Straight, cards => cards.GroupBy(card => card.Suit).Count() == 1 },
            { RankType.ThreeOfAKind, cards => cards.GroupBy(card => card.Suit).Count() == 1 },
            { RankType.TwoPair, cards => cards.GroupBy(card => card.Suit).Count() == 1 },
            { RankType.Pair, cards => cards.GroupBy(card => card.Suit).Count() == 1 },
            { RankType.HighCard, cards => cards.GroupBy(card => card.Suit).Count() == 1 },
        };

        public RankType Evalute(IEnumerable<Card> cards)
        {
            if (cards == null) throw new ArgumentNullException("No cards given");
            if (cards.Count() > 7) throw new ArgumentException("Card count must be between 1 and 7");

            return Classifier.Where(x => x.Value(cards) == true).FirstOrDefault().Key;
        }
    }
}
