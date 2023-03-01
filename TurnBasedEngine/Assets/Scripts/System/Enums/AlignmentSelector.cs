using System.Runtime.Serialization;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BF2D.Enums
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CombatAlignment
    {
        [EnumMember]
        Offense,
        [EnumMember]
        Defense,
        [EnumMember]
        Neutral
    }

    public class CombatAlignmentCollection<T>
    {
        private T[] collection;
        private readonly int enumSize = 0;

        public CombatAlignmentCollection()
        {
            enumSize = Enum.GetValues(typeof(CombatAlignment)).Length;
            collection = new T[enumSize];
        }

        public T this[CombatAlignment index]
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

    public class CombatAlignmentSelector : MonoBehaviour
    {
        public CombatAlignment combatAlignment;
    }
}