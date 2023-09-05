using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using BF2D.Game.Enums;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class CharacterActionSlot : ICombatAligned
    {
        [JsonProperty] private string gemID = string.Empty;

        [JsonProperty] private readonly NumRandInt repeat = new(0);
        [JsonProperty] private readonly HashSet<AuraType> hasAura = null;
        [JsonProperty] private readonly NumRandInt successRateModifier = new(0);
        [JsonProperty] private readonly NumRandInt critChanceModifier = new(0);
        [JsonProperty] private readonly NumRandInt exertionCostModifier = new(0);

        [JsonIgnore] protected CharacterAction Gem => GameCtx.One.GetGem(this.gemID);

        [JsonIgnore] public bool Armed => this.Gem is not null;

        [JsonIgnore] public CombatAlignment Alignment => this.Gem?.Alignment ?? CombatAlignment.Neutral;

        [JsonIgnore] public bool IsRestoration => this.Gem?.IsRestoration ?? false;

        [JsonIgnore] public bool Chaotic => this.Gem?.Chaotic ?? false;

        [JsonIgnore] public string ID { get => this.gemID; set => this.gemID = value; }

        public bool ContainsAura(AuraType aura) => this.Gem.ContainsAura(aura) || (this.hasAura?.Contains(aura) ?? false);

        public string GetAnimationKey() => this.Gem.GetAnimationKey();

        protected virtual CharacterAction.Specs Specs => new()
        {
            repeat = this.repeat,
            hasAura = this.hasAura,
            successRateModifier = this.successRateModifier,
            critChanceModifier = this.critChanceModifier,
            exertionCostModifier = this.exertionCostModifier
        };

        public CharacterAction.Info Run(CharacterStats self)
        {
            return this.Gem.Run(self, self, this.Specs);
        }

        public string TextBreakdown(CharacterStats source) => this.Gem.TextBreakdown(source, this.Specs);
    }
}
