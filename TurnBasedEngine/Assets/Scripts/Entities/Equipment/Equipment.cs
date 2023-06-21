using BF2D.Game.Enums;
using Newtonsoft.Json;
using System;
using BF2D.Game.Actions;

namespace BF2D.Game
{
    [Serializable]
    public class Equipment : PersistentEffect, IUtilityEntity
    {
        [JsonProperty] private readonly string spriteID = string.Empty;
        [JsonProperty] private readonly EquipmentType equipmentType = EquipmentType.Accessory;
        [JsonProperty] private readonly EquipModWrapper[] runes = { };

        public Entity GetEntity() => this;

        [JsonIgnore] public string SpriteID => this.spriteID;
        [JsonIgnore] public EquipmentType Type => this.equipmentType;

        [JsonIgnore] public EquipModWrapper[] Runes => this.runes;

        [JsonIgnore] public override int SpeedModifier => CalculateProperty(base.SpeedModifier, rune => rune.SpeedModifier);
        [JsonIgnore] public override int AttackModifier => CalculateProperty(base.AttackModifier, rune => rune.AttackModifier);
        [JsonIgnore] public override int DefenseModifier => CalculateProperty(base.DefenseModifier, rune => rune.DefenseModifier);
        [JsonIgnore] public override int FocusModifier => CalculateProperty(base.FocusModifier, rune => rune.FocusModifier);
        [JsonIgnore] public override int LuckModifier => CalculateProperty(base.LuckModifier, rune => rune.LuckModifier);
        [JsonIgnore] public override int MaxHealthModifier => CalculateProperty(base.MaxHealthModifier, rune => rune.MaxHealthModifier);
        [JsonIgnore] public override int MaxStaminaModifier => CalculateProperty(base.MaxStaminaModifier, rune => rune.MaxStaminaModifier);
        [JsonIgnore] public override UntargetedGameAction OnUpkeep => CalculateProperty(base.OnUpkeep, rune => rune.OnUpkeep);
        [JsonIgnore] public override UntargetedGameAction OnEOT => CalculateProperty(base.OnEOT, rune => rune.OnEOT);

        private int CalculateProperty(int baseProperty, Func<EquipModWrapper, int> runePropertyPredicate)
        {
            int runeTotal = 0;
            foreach (EquipModWrapper rune in this.runes)
                if (rune.Armed)
                    runeTotal += runePropertyPredicate(rune);
            return baseProperty + runeTotal;
        }

        private UntargetedGameAction CalculateProperty(UntargetedGameAction baseProperty, Func<EquipModWrapper, UntargetedGameAction> runePropertyPredicate)
        {
            UntargetedGameAction runeTotal = baseProperty;
            foreach (EquipModWrapper rune in this.runes)
                if (rune.Armed)
                    runeTotal = runeTotal.Append(runePropertyPredicate(rune));
            return runeTotal;
        }
    }
}
