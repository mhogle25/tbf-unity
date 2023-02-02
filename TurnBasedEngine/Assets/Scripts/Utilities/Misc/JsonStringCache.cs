using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BF2D
{
    public class JsonStringCache<T> : ICache where T : class
    {
        public JsonStringCache()
        {

        }

        public JsonStringCache(int limit)
        {
            this.cacheLimit = limit;
        }

        private readonly Dictionary<string, string> jsons = new();

        public string Datapath { get { return this.datapath; } set { this.datapath = value; } }
        private string datapath = string.Empty;

        public int CacheLimit { get { return this.cacheLimit; } set { this.cacheLimit = value; } }
        private int cacheLimit = 10;

        public T Get(string key)
        {
            if (this.jsons.ContainsKey(key))
                return BF2D.Utilities.TextFile.DeserializeString<T>(this.jsons[key]);

            string content = BF2D.Utilities.TextFile.LoadFile(Path.Combine(this.datapath, $"{key}.json"));
            if (content == string.Empty)
                return null;

            if (this.jsons.Count > this.cacheLimit)
                Clear();

            this.jsons[key] = content;

            if (this.jsons.ContainsKey(key))
                return BF2D.Utilities.TextFile.DeserializeString<T>(this.jsons[key]);

            return null;
        }

        public void Clear()
        {
            this.jsons.Clear();
        }
    }
}