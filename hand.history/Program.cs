﻿using hand.history.DataAccess;
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

            Container.RegisterType<ILogger, LoggerService>();
            Container.RegisterType<IParser, ParserService>();
            Container.RegisterType<IReader, FileReaderService>();
            Container.RegisterType<IWatcher, FileWatcherService>();
            Container.RegisterType<IEvaluator, EvaluatorService>();
            Container.RegisterType<IMapper<Table>, PokerstarsMapperService>();

            Logger = new LoggerService();

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
