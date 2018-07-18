using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace hand.history.Services.Concrete
{
    public class ConcreteDirectoryPoller : IPoller
    {
        private string Path { get; }

        public ConcreteDirectoryPoller(string path)
        {
            Path = path;
        }

        public void Run()
        {
            var watcher = new FileSystemWatcher();

            watcher.Path = Path;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.txt*";
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }

        public void OnChanged(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("File: " + e.FullPath);
        }

        public string GetChanges()
        {
            return string.Empty;
        }
    }
}
