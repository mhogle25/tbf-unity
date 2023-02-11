using System.Runtime.Serialization;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BF2D.Enums
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AuraType
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
        Restoration,
        [EnumMember]
        Evil
    }

    public class AuraTypeCollection<T>
    {
        private T[] collection;
        private readonly int enumSize = 0;

        public AuraTypeCollection()
        {
            enumSize = Enum.GetValues(typeof(AuraType)).Length;
            collection = new T[enumSize];
        }

        public T this[AuraType index]
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

    public class AuraTypeSelector : MonoBehaviour
    {
        public AuraType auraType;
    }
}