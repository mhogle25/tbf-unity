using BF2D.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;
using BF2D.Game.Enums;

namespace BF2D.Game
{
    public class StatusEffectInfo : IEntityInfo
    {

        [JsonIgnore] public string ID => this.id;
        [JsonProperty] protected string id = string.Empty;

        [JsonIgnore] public int RemainingDuration => this.remainingDuration;
        [JsonProperty] private int remainingDuration = -1;

        [JsonIgnore] public string Name => Get().Name;

        [JsonIgnore] public string Description => Get().Description;

        [JsonIgnore] public IEnumerable<AuraType> Auras => Get().Auras;

        public StatusEffectInfo(string id)
        {
            this.id = id;
            this.remainingDuration = Get().Duration;
        }

        public StatusEffect Get() => GameCtx.One.GetStatusEffect(this.ID);

        public StatusEffect Use()
        {
            if (this.remainingDuration < 1)
                return Get();

            this.remainingDuration--;
            return Get();
        }

        public int GetModifier(CharacterStatsProperty property) => Get().GetModifier(property);

        public bool ContainsAura(AuraType aura) => Get().ContainsAura(aura);
    }
}