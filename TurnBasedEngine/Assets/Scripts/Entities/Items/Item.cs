using System;
using Newtonsoft.Json;
using BF2D.Game.Actions;
using BF2D.Game.Enums;

namespace BF2D.Game
{
    [Serializable]
    public class Item : Entity, IUtilityEntity
    {
        [JsonIgnore] public override string ID { get => this.id; set => this.id = value; }
        [JsonIgnore] private string id = string.Empty;

        [JsonIgnore] public string SpriteID { get => this.spriteID; }
        [JsonIgnore] public bool Consumable { get => this.consumable; }
        [JsonIgnore] public TargetedGameAction OnUse { get => this.onUse; }
        [JsonIgnore] public CombatAlignment Alignment => this.OnUse?.Alignment ?? CombatAlignment.Neutral;

        [JsonProperty] protected readonly bool consumable = true;
        [JsonProperty] protected readonly string spriteID = string.Empty;
        [JsonProperty] protected readonly TargetedGameAction onUse = null;

        [JsonIgnore] public bool Useable { get => this.OnUse is not null; }
        [JsonIgnore] public bool CombatExclusive { get => this.Useable && this.OnUse.CombatExclusive; }

        public string TextBreakdown(CharacterStats source)
        {
            string description = $"{base.Description}\n";

            foreach (TargetedCharacterStatsAction targetedGem in this.OnUse.TargetedGems)
            {
                description += $"-\n{targetedGem.Gem.TextBreakdown(source)}";
            }
            description += "-\n";
            if (!this.Consumable)
                description += "Non-Consumable\n-\n";

            return description;
        }
    }
}
