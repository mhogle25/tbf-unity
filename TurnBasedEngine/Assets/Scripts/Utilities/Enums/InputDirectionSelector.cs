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

    public class InputDirectionSelector : MonoBehaviour
    {
        public InputDirection inputDirection;
    }
}