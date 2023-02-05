using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BF2D.Utilities
{
    public class JsonEntityCache<T> : ICache where T : Entity
    {
        public JsonEntityCache() { }

        public JsonEntityCache(int limit) => this.CacheLimit = limit;

        private readonly Dictionary<string, T> objects = new();

        public int CacheLimit { get { return this.cacheLimit; } set { this.cacheLimit = value; } }
        private int cacheLimit = 10;

        public T Get(string id, FileManager fileManager)
        {
            if (this.objects.Count > this.cacheLimit)
                Clear();

            if (this.objects.ContainsKey(id))
                return this.objects[id];

            string content = fileManager.LoadFile(id);
            if (content == string.Empty)
                return null;

            this.objects[id] = BF2D.Utilities.TextFile.DeserializeString<T>(content);

            if (this.objects.ContainsKey(id))
                return this.objects[id];

            return null;
        }

        public void Clear()
        {
            this.objects.Clear();
        }
    }
}