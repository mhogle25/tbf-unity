using Newtonsoft.Json;
using System;

namespace BF2D.Game
{
    [Serializable]
    public class StatusEffect : PersistentEffect
    {
        [JsonIgnore] public int Duration { get { return this.duration; } }
        [JsonProperty] private readonly int duration = -1;
        [JsonIgnore] public bool Singleton { get { return this.singleton; } }
        [JsonProperty] private readonly bool singleton = false;
    }
}
