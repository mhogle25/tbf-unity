using BF2D.Game.Enums;
using Newtonsoft.Json;
using System;
using BF2D.Game.Actions;

namespace BF2D.Game
{
    [Serializable]
    public class Equipment : PersistentEffect, IUtilityEntity
    {
        [JsonIgnore] public string SpriteID => this.spriteID;
        [JsonIgnore] public EquipmentType Type => this.equipmentType;

        [JsonProperty] private readonly string spriteID = string.Empty;
        [JsonProperty] private readonly EquipmentType equipmentType = EquipmentType.Accessory;

        public Entity GetEntity() => this;

        [JsonIgnore] public override int SpeedModifier => base.SpeedModifier;
        [JsonIgnore] public override int AttackModifier => base.AttackModifier;
        [JsonIgnore] public override int DefenseModifier => base.DefenseModifier;
        [JsonIgnore] public override int FocusModifier => base.FocusModifier;
        [JsonIgnore] public override int LuckModifier => base.LuckModifier;
        [JsonIgnore] public override int MaxHealthModifier => base.MaxHealthModifier;
        [JsonIgnore] public override int MaxStaminaModifier => base.MaxStaminaModifier;
        [JsonIgnore] public override UntargetedGameAction OnUpkeep => base.OnUpkeep;
        [JsonIgnore] public override UntargetedGameAction OnEOT => base.OnEOT;
    }
}
