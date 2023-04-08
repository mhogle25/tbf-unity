using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using BF2D.Game.Actions;

namespace BF2D.Game
{
    [Serializable]
    public class Item : Entity, IUtilityEntity
    {
        [JsonIgnore] public string SpriteID { get => this.spriteID; }
        [JsonProperty] protected readonly string spriteID = string.Empty;
        [JsonIgnore] public bool Consumable { get => this.consumable; }
        [JsonProperty] protected readonly bool consumable = true;
        [JsonIgnore] public Enums.CombatAlignment Alignment { get => this.alignment; }
        [JsonProperty] private readonly Enums.CombatAlignment alignment = Enums.CombatAlignment.Neutral;
        [JsonIgnore] public TargetedGameAction OnUse { get => this.onUse; }
        [JsonProperty] protected readonly TargetedGameAction onUse = null;

        [JsonIgnore] public bool Useable { get => this.OnUse is not null; }
        [JsonIgnore] public bool CombatExclusive { get => this.Useable && this.OnUse.CombatExclusive; } 

        public string TextBreakdown(CharacterStats source)
        {
            string description = $"{base.Description}\n";

            foreach (TargetedCharacterStatsAction targetedGem in this.OnUse.TargetedGems)
            {
                description += "-\n" + targetedGem.Gem.TextBreakdown(source);
            }
            description += "-\n";
            if (!this.Consumable)
                description += "Non-Consumable\n-\n";

            return description;
        }
    }
}
