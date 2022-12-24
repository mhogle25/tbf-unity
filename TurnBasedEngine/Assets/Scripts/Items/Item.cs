using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using BF2D.Game.Actions;

namespace BF2D.Game
{
    [Serializable]
    public class Item
    {
        [JsonIgnore] public string Name { get { return this.name; } }
        [JsonProperty] protected readonly string name = string.Empty;
        [JsonIgnore] public string Icon { get { return this.icon; } }
        [JsonProperty] protected readonly string icon = string.Empty;
        [JsonIgnore] public bool Consumeable { get { return this.consumeable; } }
        [JsonProperty] protected readonly bool consumeable = false;
        [JsonIgnore] public string Description { get { return this.description; } }
        [JsonProperty] protected readonly string description = string.Empty;
        [JsonIgnore] public List<string> UseMessage { get { return this.useMessage; } }
        [JsonProperty] protected readonly List<string> useMessage = new List<string>();
        [JsonIgnore] public GameAction OnUse { get { return this.onUse; } }
        [JsonProperty] protected readonly GameAction onUse = null;
    }
}
