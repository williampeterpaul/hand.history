using hand.history.Models;
using hand.history.Services;
using hand.history.Services.Concrete;
using System;
using System.Collections.Generic;
using System.IO;
using static hand.history.Models.Card;

namespace hand.history
{
    public class Program
    {
        public IWatcher Poller { get; }
        public IReader Reader { get; }
        public IParser Parser { get; }
        public ILogger Logger { get; }

        public Program(IWatcher poller, IReader reader, IParser parser, ILogger logger)
        {
            // check for updates
            // read new updates
            // parse updates to summary
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/PokerStars.UK/HandHistory/WI7ZZ";

            var poller = new DirectoryWatcher();

            poller.Run(directory);

            while (Console.Read() != 'q') ;
        }
    }
}
