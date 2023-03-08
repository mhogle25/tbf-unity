using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using BF2D.Game.Actions;

namespace BF2D.Game
{
    [Serializable]
    public class Item : Entity, ISpriteID, IUtilityEntity
    {
        [JsonIgnore] public string SpriteID { get { return this.spriteID; } }
        [JsonProperty] protected readonly string spriteID = string.Empty;
        [JsonIgnore] public bool Consumeable { get { return this.consumeable; } }
        [JsonProperty] protected readonly bool consumeable = true;
        [JsonIgnore] public Enums.CombatAlignment Alignment { get { return this.alignment; } }
        [JsonProperty] private Enums.CombatAlignment alignment = Enums.CombatAlignment.Neutral;
        [JsonIgnore] public TargetedGameAction OnUse { get { return this.onUse; } }
        [JsonProperty] protected readonly TargetedGameAction onUse = null;
    }
}
