using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using BF2D.Game.Enums;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class UntargetedCharacterStatsAction : ICombatAligned
    {
        [JsonProperty] private string gemID = string.Empty;

        [JsonProperty] private readonly NumRandInt repeat = new(1);
        [JsonProperty] private readonly AuraType? hasAura = null;
        [JsonProperty] private readonly NumRandInt successRateModifier = new(0);
        [JsonProperty] private readonly NumRandInt critChanceModifier = new(0);

        [JsonIgnore] protected CharacterStatsAction Gem => GameCtx.Instance.GetGem(this.gemID);

        [JsonIgnore] public bool Armed => this.Gem is not null;

        public bool ContainsAura(AuraType aura) => this.Gem.ContainsAura(aura);

        [JsonIgnore] public CombatAlignment Alignment => this.Gem?.Alignment ?? CombatAlignment.Neutral;

        [JsonIgnore] public string ID => this.Gem.ID;

        public void SetGemID(string newId) => this.gemID = newId;

        public string GetAnimationKey() => this.Gem.GetAnimationKey();

        protected virtual CharacterStatsAction.Specs Specs => new()
        {
            repeat = this.repeat,
            hasAura = this.hasAura,
            successRateModifier = this.successRateModifier,
            critChanceModifier = this.critChanceModifier
        };

        public CharacterStatsAction.Info Run(CharacterStats self) => this.Gem.Run(self, self, this.Specs);

        public string TextBreakdown(CharacterStats source) => this.Gem.TextBreakdown(source, this.Specs);
    }
}
