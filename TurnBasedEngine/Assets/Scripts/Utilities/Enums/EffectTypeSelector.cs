using System.Runtime.Serialization;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BF2D.Enums
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EffectType
    {
        [EnumMember]
        Generic,
        [EnumMember]
        Fire,
        [EnumMember]
        Ice,
        [EnumMember]
        Poison,
        [EnumMember]
        Psychic,
        [EnumMember]
        Evil
    }

    public class EffectTypeCollection<T>
    {
        private T[] collection;
        private readonly int enumSize = 0;

        public EffectTypeCollection()
        {
            enumSize = Enum.GetValues(typeof(EffectType)).Length;
            collection = new T[enumSize];
        }

        public T this[EffectType index]
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

    public class EffectTypeSelector : MonoBehaviour
    {
        public EffectType effectType;
    }
}