using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using BF2D.Game.Enums;

namespace BF2D.Game.Actions
{
    public class CharacterActionInfo : UtilityEntityInfo
    {
        [JsonIgnore] public override string Name => Get().Name;
        [JsonIgnore] public override string Description => Get().Description;
        [JsonIgnore] public override IEnumerable<AuraType> Auras => Get().Auras;
        [JsonIgnore] public bool HasStatsUp => Get().HasStatsUp;

        public CharacterAction Get() => GameCtx.One.GetGem(this.ID);
        protected override IUtilityEntity GetUtility() => Get();
        public override bool ContainsAura(AuraType aura) => Get().ContainsAura(aura);
    }
}
