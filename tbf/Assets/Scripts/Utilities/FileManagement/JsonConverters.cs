using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine;
using System;

namespace BF2D.Utilities
{
    public static class JsonConverters
    {
        public class HashSetConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(HashSet<string>);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject jo = JObject.Load(reader);
                return new HashSet<string>(jo.Properties().Select(p => p.Name));
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                HashSet<string> hashSet = (HashSet<string>)value;
                JObject jo = new JObject(hashSet.Select(s => new JProperty(s, s)));
                jo.WriteTo(writer);
            }
        }
    }
}
