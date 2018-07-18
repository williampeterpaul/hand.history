using hand.history.Models;
using hand.history.Services;
using hand.history.Services.Concrete;
using System;
using System.Collections.Generic;
using static hand.history.Models.Card;

namespace hand.history
{
    public class Program
    {
        public IPoller Poller { get; }
        public IReader Reader { get; }
        public IParser Parser { get; }
        public ILogger Logger { get; }

        public Program(IPoller poller, IReader reader, IParser parser, ILogger logger)
        {
            // check for updates
            // read new updates
            // parse updates to summary
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var evaluator = new ConcreteHoldemEvaluator();

            var list = new List<Card> {
                new Card { Suit = SuitType.Spade, Rank = RankType.Seven },
                new Card { Suit = SuitType.Spade, Rank = RankType.Jack },
                new Card { Suit = SuitType.Heart, Rank = RankType.Two },
                new Card { Suit = SuitType.Heart, Rank = RankType.Five },
                new Card { Suit = SuitType.Heart, Rank = RankType.Three },
                new Card { Suit = SuitType.Spade, Rank = RankType.Four },
                new Card { Suit = SuitType.Spade, Rank = RankType.Ten },
            };

            var result = evaluator.Evalute(list);

            Console.WriteLine(result);
        }
    }
}
