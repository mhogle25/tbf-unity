using System.Collections.Generic;

namespace BF2D.Utilities
{
    public class JsonEntityCache<T> : ICache where T : Entity
    {
        public JsonEntityCache() { }

        public JsonEntityCache(int limit) => this.CacheLimit = limit;

        private readonly Dictionary<string, T> entities = new();

        public int CacheLimit { get => this.cacheLimit; set => this.cacheLimit = value; }
        private int cacheLimit = 10;

        public T Get(string id, FileManager fileManager)
        {
            if (this.entities.Count > this.cacheLimit)
                Clear();

            if (this.entities.ContainsKey(id))
                return this.entities[id];

            string content = fileManager.LoadFile(id);
            if (string.IsNullOrEmpty(content))
                return null;

            this.entities[id] = JSON.DeserializeString<T>(content).Setup<T>(id);

            if (this.entities.ContainsKey(id))
                return this.entities[id];

            return null;
        }

        public void Clear()
        {
            this.entities.Clear();
        }
    }
}