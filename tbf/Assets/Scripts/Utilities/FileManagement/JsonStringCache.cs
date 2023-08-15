using System.Collections.Generic;

namespace BF2D.Utilities
{
    public class JsonStringCache<T> : ICache where T : Entity
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
                return JSON.DeserializeString<T>(this.jsons[id]).Setup<T>(id);

            string json = fileManager.LoadFile(id);
            if (string.IsNullOrEmpty(json))
                return null;

            this.jsons[id] = json;

            if (this.jsons.ContainsKey(id))
                return JSON.DeserializeString<T>(this.jsons[id]).Setup<T>(id);

            return null;
        }

        public void Clear()
        {
            this.jsons.Clear();
        }
    }
}