using System.Runtime.Serialization;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BF2D.Enums
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GameDirectory
    {
        [EnumMember]
        Persistent,
        [EnumMember]
        Streaming
    }

    public class GameDirectoryCollection<T>
    {
        private T[] collection;
        private readonly int enumSize = 0;

        public GameDirectoryCollection()
        {
            enumSize = Enum.GetValues(typeof(GameDirectory)).Length;
            collection = new T[enumSize];
        }

        public T this[GameDirectory index]
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

    public class GameDirectorySelector : MonoBehaviour
    {
        public GameDirectory gameDirectory;
    }
}