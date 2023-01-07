using System.Runtime.Serialization;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BF2D.UI.Enums
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UIState
    {
        [EnumMember]
        Standby,
        [EnumMember]
        Running,
        [EnumMember]
        Idle
    }

    public class UIStateCollection<T>
    {
        private readonly T[] collection;
        private readonly int enumSize = 0;

        public UIStateCollection()
        {
            enumSize = Enum.GetValues(typeof(UIState)).Length;
            collection = new T[enumSize];
        }

        public T this[UIState index]
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

    public class StateSelector : MonoBehaviour
    {
        public UIState state;
    }
}