using System.Runtime.Serialization;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BF2D.Game.Enums
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GemType
    {
        [EnumMember]
        Triggered,
        [EnumMember]
        Activated
    }

    public class GemTypeCollection<T>
    {
        private T[] collection;
        private readonly int enumSize = 0;

        public GemTypeCollection()
        {
            enumSize = Enum.GetValues(typeof(GemType)).Length;
            collection = new T[enumSize];
        }

        public T this[GemType index]
        {
            get
            {
                return this.collection[(int)index];
            }

            set
            {
                this.collection[(int)index] = value;
            }
        }
    }

    public class GemTypeSelector : MonoBehaviour
    {
        public GemType gemType;
    }
}