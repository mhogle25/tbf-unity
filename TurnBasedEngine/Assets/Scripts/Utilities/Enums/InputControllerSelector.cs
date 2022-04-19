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

    public class InputControllerSelector : MonoBehaviour
    {
        public InputDirection inputDirection;
    }
}