using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace hand.history.Services.Concrete
{
    public sealed class DirectoryWatcher : IWatcher
    {
        private IReader Reader { get; }

        public DirectoryWatcher(IReader reader)
        {
            Reader = reader;
        }

        public void Run(string path)
        {
            var watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.Size;
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e) => Console.WriteLine(Reader.Read(e.FullPath));
    }
}
