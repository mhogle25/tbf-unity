using System.Runtime.Serialization;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BF2D.Enums
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum InputDirection
    {
        [EnumMember]
        Up,
        [EnumMember]
        Left,
        [EnumMember]
        Down,
        [EnumMember]
        Right
    }

    public class InputDirectionCollection<T>
    {
        private T[] collection;
        private readonly int enumSize = 0;

        public InputDirectionCollection()
        {
            enumSize = Enum.GetValues(typeof(InputDirection)).Length;
            collection = new T[enumSize];
        }

        public T this[InputDirection index]
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

    public class InputDirectionSelector : MonoBehaviour
    {
        public InputDirection inputDirection;
    }
}