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

    public class InputButtonSelector : MonoBehaviour
    {
        public InputButton inputButton;
    }
}