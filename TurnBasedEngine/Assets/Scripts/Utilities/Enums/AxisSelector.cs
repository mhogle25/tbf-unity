using System.Runtime.Serialization;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BF2D.Enums
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Axis
    {
        [EnumMember]
        Horizontal,
        [EnumMember]
        Vertical
    };

    public class AxisCollection<T>
    {
        private T[] collection;
        private readonly int enumSize = 0;

        public AxisCollection()
        {
            enumSize = Enum.GetValues(typeof(Axis)).Length;
            collection = new T[enumSize];
        }

        public T this[Axis index]
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

    public class AxisSelector : MonoBehaviour
    {
        public Axis axis;
    }
}
