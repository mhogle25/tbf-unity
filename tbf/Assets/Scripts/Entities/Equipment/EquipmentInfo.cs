using System;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;
using BF2D.Enums;
using BF2D.Game.Enums;

namespace BF2D.Game
{
    [Serializable]
    public class EquipmentInfo : UtilityEntityInfo
    {
        [JsonIgnore] public override string Name => Get().Name;

        [JsonIgnore] public override string Description => Get().Description;

        [JsonIgnore] public EquipmentType Type => Get().Type;

        public Equipment Get() => GameCtx.One.GetEquipment(this.ID);

        protected override IUtilityEntity GetUtility() => Get();

        public override IEnumerable<AuraType> Auras => Get().Auras;

        public override bool ContainsAura(AuraType aura) => Get().ContainsAura(aura);
    }
}
