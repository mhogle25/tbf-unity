using System;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;
using BF2D.Enums;

namespace BF2D.Game
{
    [Serializable]
    public class EquipmentInfo : UtilityEntityInfo
    {
        [JsonIgnore] public override Sprite Icon => GameCtx.Instance.GetIcon(this.GetUtility().SpriteID);

        [JsonIgnore] public override string Description => Get().Description;

        [JsonIgnore] public override IEnumerable<Enums.AuraType> Auras => Get().Auras;

        public Equipment Get() => GameCtx.Instance.GetEquipment(this.ID);

        public override Entity GetEntity() => Get();

        public override IUtilityEntity GetUtility() => Get();
    }
}
