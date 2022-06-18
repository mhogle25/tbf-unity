using System.Runtime.Serialization;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BF2D.Enums
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum InputButton
    {
        [EnumMember]
        Confirm,
        [EnumMember]
        Back,
        [EnumMember]
        Menu,
        [EnumMember]
        Attack,
        [EnumMember]
        Pause,
        [EnumMember]
        Select
    }

    public class InputButtonCollection<T>
    {
        private T[] collection;
        private readonly int enumSize = 0;

        public InputButtonCollection()
        {
            enumSize = Enum.GetValues(typeof(InputButton)).Length;
            collection = new T[enumSize];
        }

        public T this[InputButton index]
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

    public class InputButtonSelector : MonoBehaviour
    {
        public InputButton inputButton;
    }
}