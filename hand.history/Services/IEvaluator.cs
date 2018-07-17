using hand.history.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Services.Concrete
{
    public interface IEvaluator<TCards> where TCards : IEnumerable<Card>
    {
        Hand.RankType Evalute(TCards cards);
    }
}
