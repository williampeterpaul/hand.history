using hand.history.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hand.history.Services.Concrete
{
    public class ConcreteHoldemEvaluator : IEvaluator<IEnumerable<Card>>
    {
        private readonly IReadOnlyDictionary<Hand.RankType, Func<IEnumerable<Card>, bool>> Classifier = new Dictionary<Hand.RankType, Func<IEnumerable<Card>, bool>>
        {
            { Hand.RankType.RoyalFlush, IsFlush },
            { Hand.RankType.StraightFlush, IsFlush },
            { Hand.RankType.FourOfAKind, IsFlush },
            { Hand.RankType.FullHouse, IsFlush },
            { Hand.RankType.Flush, IsFlush },
            { Hand.RankType.Straight, IsFlush },
            { Hand.RankType.ThreeOfAKind, IsFlush },
            { Hand.RankType.TwoPair, IsFlush },
            { Hand.RankType.Pair, IsFlush },
            { Hand.RankType.HighCard, IsFlush },
        };

        private static Func<IEnumerable<Card>, bool> IsFlush = cards => cards.GroupBy(card => card.Suit).Count() == 1;

        public Hand.RankType Evalute(IEnumerable<Card> cards)
        {



            throw new NotImplementedException();
        }
    }
}
