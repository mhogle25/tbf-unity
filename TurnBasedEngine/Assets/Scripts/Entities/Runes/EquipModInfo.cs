using System.Collections;
using System.Collections.Generic;
using BF2D.Game.Actions;
using Newtonsoft.Json;
using UnityEngine;

namespace BF2D.Game
{
    public class EquipModInfo : UtilityEntityInfo
    {
        [JsonIgnore] public override Sprite Icon { get; }

        [JsonIgnore] public override string Description => Get().Description;

        [JsonIgnore] public override IEnumerable<Enums.AuraType> Auras => Get().Auras;

        [JsonIgnore] public int SpeedModifier => Get().SpeedModifier;
        [JsonIgnore] public int AttackModifier => Get().AttackModifier;
        [JsonIgnore] public int DefenseModifier => Get().DefenseModifier;
        [JsonIgnore] public int FocusModifier => Get().FocusModifier;
        [JsonIgnore] public int LuckModifier => Get().LuckModifier;
        [JsonIgnore] public int MaxHealthModifier => Get().MaxHealthModifier;
        [JsonIgnore] public int MaxStaminaModifier => Get().MaxStaminaModifier;
        [JsonIgnore] public virtual UntargetedGameAction OnUpkeep => Get().OnUpkeep;
        [JsonIgnore] public virtual UntargetedGameAction OnEOT => Get().OnEOT;

        public bool UpkeepEventExists() => Get().UpkeepEventExists();
        public bool EOTEventExists() => Get().EOTEventExists();

        public EquipMod Get()
        {
            return GameCtx.Instance.GetRune(this.ID);
        }

        public override Entity GetEntity() => Get();

        public override IUtilityEntity GetUtility() => Get();
    }
}