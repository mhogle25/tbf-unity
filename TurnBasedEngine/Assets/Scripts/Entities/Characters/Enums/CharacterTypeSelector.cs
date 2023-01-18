using System.Runtime.Serialization;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BF2D.Enums
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CharacterType
    {
        [EnumMember]
        Player,
        [EnumMember]
        Enemy
    }

    public class CharacterTypeCollection<T>
    {
        private readonly T[] collection;
        private readonly int enumSize = 0;

        public CharacterTypeCollection()
        {
            enumSize = Enum.GetValues(typeof(CharacterType)).Length;
            collection = new T[enumSize];
        }

        public T this[CharacterType index]
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

    public class CharacterTypeSelector : MonoBehaviour
    {
        public CharacterType characterType;
    }
}