using System.Runtime.Serialization;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BF2D.Game.Enums
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EquipmentType
    {
        [EnumMember]
        Accessory,
        [EnumMember]
        Head,
        [EnumMember]
        Torso,
        [EnumMember]
        Legs,
        [EnumMember]
        Hands,
        [EnumMember]
        Feet
    }

    public class EquipmentTypeCollection<T>
    {
        private readonly T[] collection;
        private readonly int enumSize = 0;

        public EquipmentTypeCollection()
        {
            enumSize = Enum.GetValues(typeof(EquipmentType)).Length;
            collection = new T[enumSize];
        }

        public T this[EquipmentType index]
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

    public class EquipmentTypeSelector : MonoBehaviour
    {
        public EquipmentType equipmentType;
    }
}