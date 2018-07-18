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
        public IWatcher Watcher { get; }
        public IReader Reader { get; }
        public IParser Parser { get; }
        public ILogger Logger { get; }

        public Program(IWatcher watcher, IReader reader, IParser parser, ILogger logger)
        {
            // check for updates
            // read new updates
            // parse updates to summary
        }

        public static void Main(string[] args)
        {
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/PokerStars.UK/HandHistory/WI7ZZ";
            var watcher = new DirectoryWatcher();

            watcher.Run(directory);

            while (Console.Read() != 0) ;
        }
    }
}
