using hand.history.Data;
using hand.history.Extensions;
using hand.history.Models;
using hand.history.Services;
using hand.history.Services.Concrete;
using System;
using System.Linq;
using Unity;

namespace hand.history
{
    public class Program
    {
        private UnityContainer Container { get; }

        private ILogger Logger { get; set; }

        public Program()
        {
            Container = new UnityContainer();

            Container.RegisterType<ILogger, Logger>();
            Container.RegisterType<IParser, Parser>();
            Container.RegisterType<IReader, FileReader>();
            Container.RegisterType<IWatcher, FileWatcher>();
            Container.RegisterType<IEvaluator, Evaluator>();
            Container.RegisterType<IMapper<Table>, PokerstarsMapper>();

            //Logger = Container.Resolve<Logger>();

            //Logger.LogInformation("Hello world!", new { Test = "Test", Whatever = "Another Test" });

            var directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/PokerStars.UK/HandHistory/WI7ZZ";

            ApplicationContext context = new ApplicationContext();

            context.Database.EnsureCreated();
        }

        public void MapExampleFile()
        {
            var example = Environment.CurrentDirectory + "/HH20180715 Aludra - $0.05-$0.10 - USD No Limit Hold'em.txt";

            var data = new FileReader().Read(example);
            var split = data.Split(Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine).First();

            var mapper = Container.Resolve<IMapper<Table>>().Map(split.Split(Environment.NewLine));
        }

        public static void Main(string[] args)
        {

            Program program = new Program();

            program.MapExampleFile();

            Console.ReadKey();
        }
    }
}
