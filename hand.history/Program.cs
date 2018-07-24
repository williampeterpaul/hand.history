using hand.history.Data;
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
        public IMapper<Table> Mapper { get; }
        public ILogger Logger { get; }
        public IEvaluator Evaluator { get; }

        public Program(IWatcher watcher, IMapper<Table> mapper, ILogger logger, IEvaluator evaluator)
        {
            Watcher = watcher;
            Mapper = mapper;
            Logger = logger;
            Evaluator = evaluator;

            Logger.LogInformation("Hello world!", new { Test = "Test", Whatever = "Another Test" });

            var directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/PokerStars.UK/HandHistory/WI7ZZ";

            ApplicationContext context = new ApplicationContext();

            context.Database.EnsureCreated();
        }

        private void MapExampleFile()
        {
            var example = Environment.CurrentDirectory + "/HH20180715 Aludra - $0.05-$0.10 - USD No Limit Hold'em.txt";

            var data = new FileReader().Read(example).Split(Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine).First();
            var mapper = new PokerstarsMapper(new Parser()).Map(data.Split(Environment.NewLine));
        }

        public static void Main(string[] args)
        {

            Program program = new Program(new FileWatcher(new FileReader()), new PokerstarsMapper(new Parser()), new Logger(), new Evaluator());
            Console.ReadKey();
        }
    }
}
