﻿using hand.history.Data;
using hand.history.Extensions;
using hand.history.Models;
using hand.history.Services;
using hand.history.Services.Concrete;
using System;
using System.Linq;

namespace hand.history
{
    public class Program
    {
        public IWatcher Watcher { get; }
        public IReader Reader { get; }
        public IParser Parser { get; }
        public IMapper<Table> Mapper { get; }
        public ILogger Logger { get; }
        public IEvaluator Evaluator { get; }

        public Program(IWatcher watcher, IReader reader, IParser parser, IMapper<Table> mapper, ILogger logger, IEvaluator evaluator)
        {
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/PokerStars.UK/HandHistory/WI7ZZ";

            Watcher = watcher;
            Reader = reader;
            Parser = parser;
            Mapper = mapper;
            Logger = logger;
            Evaluator = evaluator;
        }

        public static void Main(string[] args)
        {
            var logger = new Logger();
            logger.LogInformation("Hello world!", new { Test = "Test", Whatever = "Another Test" });

            var file = Environment.CurrentDirectory + "/HH20180715 Aludra - $0.05-$0.10 - USD No Limit Hold'em.txt";

            //var data = new FileReader().Read(file).Split(Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine).First();

            //var mapper = new PokerstarsMapper(new Parser(), data).Map();

            Context context = new Context();

            context.Database.EnsureCreated();

            Console.ReadKey();
        }
    }
}
