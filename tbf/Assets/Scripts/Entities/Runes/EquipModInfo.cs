using System.Collections;
using System.Collections.Generic;
using BF2D.Game.Actions;
using BF2D.Game.Enums;
using Newtonsoft.Json;
using UnityEngine;

namespace BF2D.Game
{
    public class EquipModInfo : UtilityEntityInfo
    {
        [JsonIgnore] public override string Name => Get().Name;

        [JsonIgnore] public override string Description => Get().Description;

        [JsonIgnore] public override IEnumerable<AuraType> Auras => Get().Auras;

        [JsonIgnore] public int SpeedModifier => Get().SpeedModifier;
        [JsonIgnore] public int AttackModifier => Get().AttackModifier;
        [JsonIgnore] public int DefenseModifier => Get().DefenseModifier;
        [JsonIgnore] public int FocusModifier => Get().FocusModifier;
        [JsonIgnore] public int LuckModifier => Get().LuckModifier;
        [JsonIgnore] public int MaxHealthModifier => Get().MaxHealthModifier;
        [JsonIgnore] public int MaxStaminaModifier => Get().MaxStaminaModifier;
        [JsonIgnore] public virtual UntargetedGameAction OnUpkeep => Get().OnUpkeep;
        [JsonIgnore] public virtual UntargetedGameAction OnEOT => Get().OnEOT;
        [JsonIgnore] public virtual bool UpkeepEventExists => Get().UpkeepEventExists;
        [JsonIgnore] public virtual bool EOTEventExists => Get().EOTEventExists;

        public EquipMod Get() => GameCtx.One.GetRune(this.ID);

        protected override IUtilityEntity GetUtility() => Get();

        public override bool ContainsAura(AuraType aura) => Get().ContainsAura(aura);
    }
}