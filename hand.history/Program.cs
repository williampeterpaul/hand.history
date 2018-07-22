using hand.history.Extensions;
using hand.history.Models;
using hand.history.Services;
using hand.history.Services.Concrete;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using static hand.history.Models.Card;

namespace hand.history
{
    public class Program
    {
        public IWatcher Watcher { get; }
        public IReader Reader { get; }
        public IParser Parser { get; }
        public ILogger Logger { get; }
        public IEvaluator Evaluator { get; }

        public Program(IWatcher watcher, IReader reader, IParser parser, ILogger logger, IEvaluator evaluator)
        {

            // check for updates
            // read new updates
            // parse updates to summary
        }

        public static void Main(string[] args)
        {
            var logger = new Logger();
            logger.LogInformation("Hello world!", new { Test = "Test", Whatever = "Another Test"});

            //var directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/PokerStars.UK/HandHistory/WI7ZZ";
            var file = Environment.CurrentDirectory + "/HH20180715 Aludra - $0.05-$0.10 - USD No Limit Hold'em.txt";

            var data = new FileReader().Read(file).Split(Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine).First();

            var mapper = new PokerstarsMapper(new Parser(), data).Map();

        }
    }
}
