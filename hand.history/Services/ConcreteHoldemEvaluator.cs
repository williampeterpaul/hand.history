using hand.history.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Services
{
    public class ConcreteHoldemEvaluator : IEvaluator<Hand, IEnumerable<Card>>
    {
        public Hand.RankType Evalute(Hand hand, IEnumerable<Card> cards)
        {
            throw new NotImplementedException();
        }
    }
}
