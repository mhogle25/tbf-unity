using System;
using Newtonsoft.Json;

namespace BF2D.Game.Actions
{
    [Serializable]
    public struct StatsAction
    {
        [JsonIgnore] public bool MultiTarget { get { return this.multiTarget; } }
        [JsonProperty] private bool multiTarget;

        private decimal i;
    }
}
