using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BF2D.Utilities
{
    public class JsonStringCache<T> : ICache where T : class
    {
        public JsonStringCache() { }

        public JsonStringCache(int limit) => this.CacheLimit = limit;

        private readonly Dictionary<string, string> jsons = new();

        public int CacheLimit { get { return this.cacheLimit; } set { this.cacheLimit = value; } }
        private int cacheLimit = 10;

        public T Get(string id, FileManager fileManager)
        {
            if (this.jsons.Count > this.cacheLimit)
                Clear();

            if (this.jsons.ContainsKey(id))
                return BF2D.Utilities.TextFile.DeserializeString<T>(this.jsons[id]);

            string content = fileManager.LoadFile(id);
            if (content == string.Empty)
                return null;

            this.jsons[id] = content;

            if (this.jsons.ContainsKey(id))
                return BF2D.Utilities.TextFile.DeserializeString<T>(this.jsons[id]);

            return null;
        }

        public void Clear()
        {
            this.jsons.Clear();
        }
    }
}