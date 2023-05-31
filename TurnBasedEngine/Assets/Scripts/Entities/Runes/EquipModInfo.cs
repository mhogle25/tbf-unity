using System.Collections;
using System.Collections.Generic;
using BF2D.Game.Actions;
using Newtonsoft.Json;
using UnityEngine;

namespace BF2D.Game
{
    public class EquipModInfo : IUtilityEntityInfo
    {
        [JsonIgnore] public string ID => this.id;
        [JsonProperty] private string id = string.Empty;

        [JsonIgnore] public int Count => this.count;
        [JsonProperty] private int count = 0;

        [JsonIgnore] public Sprite Icon { get; }

        [JsonIgnore] public string Name => Get().Name;

        [JsonIgnore] public string Description => Get().Description;

        [JsonIgnore] public IEnumerable<Enums.AuraType> Auras => Get().Auras;

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

        public Entity GetEntity() => Get();

        public IUtilityEntity GetUtility() => Get();

        public int Increment()
        {
            return ++this.count;
        }

        public int Decrement()
        {
            return --this.count;
        }
    }
}