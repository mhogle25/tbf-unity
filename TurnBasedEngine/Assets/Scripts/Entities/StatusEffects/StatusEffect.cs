using Newtonsoft.Json;
using System;

namespace BF2D.Game
{
    [Serializable]
    public class StatusEffect : PersistentEffect
    {
        [JsonIgnore] public int Duration => this.duration;
        [JsonIgnore] public bool Singleton => this.singleton;

        [JsonProperty] private readonly int duration = -1;
        [JsonProperty] private readonly bool singleton = false;

    }
}
