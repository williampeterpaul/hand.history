using hand.history.DataAccess;
using hand.history.Extensions;
using hand.history.DataObject;
using hand.history.Services;
using System;
using System.Linq;
using Unity;
using hand.history.Services.Interfaces;

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

            Logger = new Logger();

            Logger.LogInformation("Hello world!", new { Test = "Test", Whatever = "Another Test" });

            var directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/PokerStars.UK/HandHistory/WI7ZZ";

            ApplicationContext context = new ApplicationContext();

            context.Database.EnsureCreated();
        }

        public void MapExampleFile()
        {
            var example = Environment.CurrentDirectory + "/HH20180715 Aludra - $0.05-$0.10 - USD No Limit Hold'em.txt";

            var reader = Container.Resolve<IReader>();
            var mapper = Container.Resolve<IMapper<Table>>();

            var data = reader.Read(example).Split(Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine).First();
            var map = mapper.Map(data.Split(Environment.NewLine));
        }

        public static void Main(string[] args)
        {

            Program program = new Program();

            program.MapExampleFile();

            Console.ReadKey();
        }
    }
}
