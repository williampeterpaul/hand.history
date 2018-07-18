using hand.history.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static hand.history.Models.Hand;

namespace hand.history.Services
{
    public interface IEvaluator<TCards> where TCards : IEnumerable<Card>
    {
        RankType Evalute(TCards cards);
    }
}
