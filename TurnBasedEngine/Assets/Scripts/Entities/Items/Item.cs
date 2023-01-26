using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using BF2D.Game.Actions;

namespace BF2D.Game
{
    [Serializable]
    public class Item : Entity, ISpriteID
    {
        [JsonIgnore] public string SpriteID { get { return this.spriteID; } }
        [JsonProperty] protected readonly string spriteID = string.Empty;
        [JsonIgnore] public bool Consumeable { get { return this.consumeable; } }
        [JsonProperty] protected readonly bool consumeable = false;
        [JsonIgnore] public TargetedGameAction OnUse { get { return this.onUse; } }
        [JsonProperty] protected readonly TargetedGameAction onUse = null;
    }
}
