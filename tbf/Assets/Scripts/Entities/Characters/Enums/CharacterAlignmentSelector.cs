using System.Runtime.Serialization;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BF2D.Game.Enums
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CharacterAlignment
    {
        [EnumMember]
        Player,
        [EnumMember]
        Enemy,
        [EnumMember]
        NPC
    }

    public class CharacterAlignmentCollection<T>
    {
        private T[] collection;
        private readonly int enumSize = 0;

        public CharacterAlignmentCollection()
        {
            this.enumSize = Enum.GetValues(typeof(CharacterAlignment)).Length;
            this.collection = new T[this.enumSize];
        }

        public T this[CharacterAlignment index]
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

    public class CharacterAlignmentSelector : MonoBehaviour
    {
        public CharacterAlignment characterAlignment;
    }
}