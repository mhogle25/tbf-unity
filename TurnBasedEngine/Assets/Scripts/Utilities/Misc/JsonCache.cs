using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BF2D
{
    public class JsonCache<T>
    {

        private readonly Dictionary<string, T> objects = new();

        public int Count { get { return this.count; } }
        private int count = 0;

        public string Datapath { get { return this.datapath; } set { this.datapath = value; } }
        private string datapath = string.Empty;

        public T this[string key]
        {
            get
            {
                if (this.objects.ContainsKey(key))
                    return this.objects[key];

                string content = BF2D.Utilities.TextFile.LoadFile(Path.Combine(this.datapath, key + ".json"));
                T t = BF2D.Utilities.TextFile.DeserializeString<T>(content);
                this.objects[key] = t;

                if (this.objects.ContainsKey(key))
                {
                    this.count++;
                    return this.objects[key];
                }

                return default;
            }
        }
    }
}