using hand.history.DataAccess;
using hand.history.Extensions;
using hand.history.DataObject;
using hand.history.Services;
using System;
using System.Linq;
using Unity;
using hand.history.Services.Interfaces;
using System.Collections.Generic;

namespace hand.history
{
    public class Program
    {
        private UnityContainer Container { get; }
        private ApplicationContext Context { get; }

        public Program()
        {
            Container = new UnityContainer();
            Context = new ApplicationContext();

            Container.RegisterType<ILogger, LoggerService>();
            Container.RegisterType<IParser, ParserService>();
            Container.RegisterType<IReader, FileReaderService>();
            Container.RegisterType<IWatcher, FileWatcherService>();
            Container.RegisterType<IEvaluator, EvaluatorService>();
            Container.RegisterType<IMapper<Table>, PokerstarsMapperService>();

            Context.Database.EnsureCreated();
        }

        public void MapExampleFile()
        {
            var example = Environment.CurrentDirectory + "/HH20180715 Aludra - $0.05-$0.10 - USD No Limit Hold'em.txt";

            var reader = Container.Resolve<IReader>();
            var mapper = Container.Resolve<IMapper<Table>>();

            var maps = new List<Table>();

            var dataSet = reader.Read(example).Split("\n\n\n\n");
            Console.WriteLine(dataSet.Count());
            foreach (var data in dataSet)
            {
                try
                {
                    var map = mapper.Map(data.Split("\n"));
                    maps.Add(map);
                }
                catch
                {

                }
            }

            Context.Tables.AddRange(maps);
            Context.SaveChanges();
        }

        public static void Main(string[] args)
        {
            Program program = new Program();

            program.MapExampleFile();

            Console.ReadKey();
        }
    }
}
