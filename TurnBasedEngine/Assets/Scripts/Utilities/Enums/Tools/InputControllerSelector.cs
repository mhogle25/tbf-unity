using System.Runtime.Serialization;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BF2D.Enums
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum InputController
    {
        [EnumMember]
        Keyboard,
        [EnumMember]
        Gamepad
    }

    public class InputControllerCollection<T>
    {
        private T[] collection;
        private readonly int enumSize = 0;

        public InputControllerCollection()
        {
            enumSize = Enum.GetValues(typeof(InputController)).Length;
            collection = new T[enumSize];
        }

        public T this[InputController index]
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

    public class InputControllerSelector : MonoBehaviour
    {
        public InputDirection inputDirection;
    }
}