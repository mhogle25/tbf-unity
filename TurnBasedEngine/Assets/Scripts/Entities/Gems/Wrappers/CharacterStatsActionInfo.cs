using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using BF2D.Game.Enums;

namespace BF2D.Game.Actions
{
    public class CharacterStatsActionInfo : UtilityEntityInfo
    {
        [JsonIgnore] public override Sprite Icon => GameCtx.Instance.GetIcon(GetUtility().SpriteID);
        [JsonIgnore] public override string Description => GetEntity().Description;
        [JsonIgnore] public override IEnumerable<AuraType> Auras => GetEntity().Auras;
        [JsonIgnore] public bool HasStatsUp => Get().HasStatsUp;

        public CharacterStatsAction Get() => GameCtx.Instance.GetGem(this.ID);
        public override Entity GetEntity() => Get();
        public override IUtilityEntity GetUtility() => Get();
    }
}
