﻿using hand.history.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace hand.history.Services
{
    public class FileReaderService : IReader
    {
        private IDictionary<string, Stream> Streams { get; }

        public FileReaderService()
        {
            Streams = new Dictionary<string, Stream>();
        }

        public string Read(string path)
        {
            var exists = CreateStream(path);
            var stream = Streams[path];

            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private bool CreateStream(string path) => Streams.TryAdd(path, new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
    }
}
