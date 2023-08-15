using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using BF2D.Game.Enums;

namespace BF2D.Game.Actions
{
    public class CharacterStatsActionInfo : UtilityEntityInfo
    {
        [JsonIgnore] public override Sprite Icon => GameCtx.One.GetIcon(GetUtility().SpriteID);
        [JsonIgnore] public override string Name => Get().Name;
        [JsonIgnore] public override string Description => Get().Description;
        [JsonIgnore] public override IEnumerable<AuraType> Auras => Get().Auras;
        [JsonIgnore] public bool HasStatsUp => Get().HasStatsUp;

        public CharacterStatsAction Get() => GameCtx.One.GetGem(this.ID);
        public override IUtilityEntity GetUtility() => Get();
        public override bool ContainsAura(AuraType aura) => Get().ContainsAura(aura);
    }
}
