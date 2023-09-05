using System.Runtime.Serialization;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BF2D.Game.Enums
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CharacterTarget
    {
        [EnumMember]
        Self,
        [EnumMember]
        Ally,
        [EnumMember]
        AllAllies,
        [EnumMember]
        Opponent,
        [EnumMember]
        AllOpponents,
        [EnumMember]
        Any,
        [EnumMember]
        AllOfAny,
        [EnumMember]
        All,
        [EnumMember]
        RandomOpponent,
        [EnumMember]
        RandomAlly,
        [EnumMember]
        Random,
    }

    public class CharacterTargetCollection<T>
    {
        private T[] collection;
        private readonly int enumSize = 0;

        public CharacterTargetCollection()
        {
            this.enumSize = Enum.GetValues(typeof(CharacterTarget)).Length;
            this.collection = new T[this.enumSize];
        }

        public T this[CharacterTarget index]
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

    public class CharacterTargetSelector : MonoBehaviour
    {
        public CharacterTarget characterTarget;
    }
}