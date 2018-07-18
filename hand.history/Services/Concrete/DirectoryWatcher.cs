using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace hand.history.Services.Concrete
{
    public class DirectoryWatcher : IWatcher
    {
        public void Run(string path)
        {
            var watcher = new FileSystemWatcher
            {
                Path = path,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size,
                Filter = "*.txt"
            };

            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e) => GetChanges(e.FullPath);

        public string GetChanges(string path)
        {
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            using (var sr = new StreamReader(fs))
            {
                Console.WriteLine(sr.ReadToEnd());
            }

            return string.Empty;
        }
    }
}
