using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using BF2D.Actions;

namespace BF2D.Game
{
    [Serializable]
    public class Item
    {
        [JsonIgnore] public string Name { get { return this.name; } }
        [JsonProperty] protected string name = string.Empty;
        [JsonIgnore] public string Icon { get { return this.icon; } }
        [JsonProperty] protected string icon = string.Empty;
        [JsonIgnore] public bool Consumeable { get { return this.consumeable; } }
        [JsonProperty] protected bool consumeable = false;
        [JsonIgnore] public string Description { get { return this.description; } }
        [JsonProperty] protected string description = string.Empty;
        [JsonIgnore] public List<string> UseMessage { get { return this.useMessage; } }
        [JsonProperty] protected List<string> useMessage = new List<string>();
        [JsonIgnore] public GameAction OnUse { get { return this.onUse; } }
        [JsonProperty] protected GameAction onUse = null;
    }
}
