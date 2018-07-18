using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace hand.history.Services.Concrete
{
    public sealed class DirectoryWatcher : IWatcher
    {
        private IDictionary<string, Stream> Streams { get; } = new Dictionary<string, Stream>();

        public void Run(string path)
        {
            var watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.Filter = "*.txt";
            watcher.NotifyFilter = NotifyFilters.Size;
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }

        public string GetChanges(string path)
        {
            var result = SetStream(path);
            var stream = Streams[path];

            using (var reader = new StreamReader(stream))
            {
                Console.WriteLine(reader.ReadToEnd());
            }

            return string.Empty;
        }

        private void OnChanged(object source, FileSystemEventArgs e) => GetChanges(e.FullPath);

        private bool GetStream(string path, out Stream stream) => Streams.TryGetValue(path, out stream);

        private bool SetStream(string path) => Streams.TryAdd(path, new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
    }
}
