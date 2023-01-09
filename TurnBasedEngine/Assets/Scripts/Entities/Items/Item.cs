using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using BF2D.Game.Actions;

namespace BF2D.Game
{
    [Serializable]
    public class Item : Entity
    {
        [JsonIgnore] public string IconID { get { return this.iconID; } }
        [JsonProperty] protected readonly string iconID = string.Empty;
        [JsonIgnore] public bool Consumeable { get { return this.consumeable; } }
        [JsonProperty] protected readonly bool consumeable = false;
        [JsonIgnore] public TargetedGameAction OnUse { get { return this.onUse; } }
        [JsonProperty] protected readonly TargetedGameAction onUse = null;
    }
}
