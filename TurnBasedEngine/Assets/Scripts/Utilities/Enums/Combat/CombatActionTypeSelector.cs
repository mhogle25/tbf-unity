using System.Runtime.Serialization;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BF2D.Enums
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CombatActionType
    {
        [EnumMember]
        Act, //T, S
        [EnumMember]
        Equip, //S
        [EnumMember]
        Event, //T?, S?
        [EnumMember]
        Flee, 
        [EnumMember]
        Item, //T, S
        [EnumMember]
        Roster //T
    }

    public class CombatActionTypeCollection<T>
    {
        private T[] collection;
        private readonly int enumSize = 0;

        public CombatActionTypeCollection()
        {
            enumSize = Enum.GetValues(typeof(CombatActionType)).Length;
            collection = new T[enumSize];
        }

        public T this[CombatActionType index]
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

    public class CombatActionTypeSelector : MonoBehaviour
    {
        public CombatActionType combatActionType;
    }
}