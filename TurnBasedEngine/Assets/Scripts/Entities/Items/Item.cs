using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using BF2D.Game.Actions;

namespace BF2D.Game
{
    [Serializable]
    public class Item : Entity, IUtilityEntity
    {
        [JsonIgnore] public string SpriteID { get { return this.spriteID; } }
        [JsonProperty] protected readonly string spriteID = string.Empty;
        [JsonIgnore] public bool Consumable { get { return this.consumable; } }
        [JsonProperty] protected readonly bool consumable = true;
        [JsonIgnore] public Enums.CombatAlignment Alignment { get { return this.alignment; } }
        [JsonProperty] private readonly Enums.CombatAlignment alignment = Enums.CombatAlignment.Neutral;
        [JsonIgnore] public TargetedGameAction OnUse { get { return this.onUse; } }
        [JsonProperty] protected readonly TargetedGameAction onUse = null;
    }
}
