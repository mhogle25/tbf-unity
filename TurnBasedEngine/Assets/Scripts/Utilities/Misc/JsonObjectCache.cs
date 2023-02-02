using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BF2D
{
    public class JsonObjectCache<T> : ICache where T : class
    {

        public JsonObjectCache()
        {

        }

        public JsonObjectCache(int limit)
        {
            this.cacheLimit = limit;
        }

        private readonly Dictionary<string, T> objects = new();

        public string Datapath { get { return this.datapath; } set { this.datapath = value; } }
        private string datapath = string.Empty;

        public int CacheLimit { get { return this.cacheLimit; } set { this.cacheLimit = value; } }
        private int cacheLimit = 10;

        public T Get(string key)
        {
            if (this.objects.ContainsKey(key))
                return this.objects[key];

            string content = BF2D.Utilities.TextFile.LoadFile(Path.Combine(this.datapath, $"{key}.json"));
            if (content == string.Empty)
                return null;

            if (this.objects.Count > this.cacheLimit)
                Clear();

            this.objects[key] = BF2D.Utilities.TextFile.DeserializeString<T>(content);

            if (this.objects.ContainsKey(key))
                return this.objects[key];

            return null;
        }

        public void Clear()
        {
            this.objects.Clear();
        }
    }
}