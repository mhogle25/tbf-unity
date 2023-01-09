using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BF2D
{
    public class JsonCache<T>
    {

        private readonly Dictionary<string, string> objects = new();

        public string Datapath { get { return this.datapath; } set { this.datapath = value; } }
        private string datapath = string.Empty;

        public T Get(string key)
        {
            if (this.objects.ContainsKey(key))
                return BF2D.Utilities.TextFile.DeserializeString<T>(this.objects[key]);

            string content = BF2D.Utilities.TextFile.LoadFile(Path.Combine(this.datapath, $"{key}.json"));
            this.objects[key] = content;

            if (this.objects.ContainsKey(key))
            {
                return BF2D.Utilities.TextFile.DeserializeString<T>(this.objects[key]);
            }

            return default;
        }
    }
}