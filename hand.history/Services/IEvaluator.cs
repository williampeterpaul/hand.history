using hand.history.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Services
{
    public interface IEvaluator<THand, TCards> 
        where THand : Hand 
        where TCards : IEnumerable<Card>
    {
        Hand.RankType Evalute(THand hand, TCards cards);
    }
}
