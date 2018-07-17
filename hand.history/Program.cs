using hand.history.Services;
using System;

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
        }
    }
}
