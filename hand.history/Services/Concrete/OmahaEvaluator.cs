using hand.history.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static hand.history.Models.Hand;

namespace hand.history.Services.Concrete
{
    public sealed class OmahaEvaluator : IEvaluator<IEnumerable<Card>>
    {
        public RankType Evalute(IEnumerable<Card> cards)
        {
            throw new NotImplementedException();
        }
    }
}
