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

    public class AxisSelector : MonoBehaviour
    {
        public Axis axis;
    }
}
