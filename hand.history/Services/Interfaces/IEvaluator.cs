using hand.history.DataObject;
using System;
using System.Collections.Generic;
using System.Text;
using static hand.history.DataObject.Hand;

namespace hand.history.Services.Interfaces
{
    public interface IEvaluator
    {
        RankType Evalute(IEnumerable<Card> cards);
    }
}
