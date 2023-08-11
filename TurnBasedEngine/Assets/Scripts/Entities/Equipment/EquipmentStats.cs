using System;
using Newtonsoft.Json;
using BF2D.Game.Enums;
using UnityEngine;

namespace BF2D.Game
{
    [Serializable]
    public class EquipmentStats
    {
        [JsonProperty] private string accessory = null;
        [JsonProperty] private string head = null;
        [JsonProperty] private string torso = null;
        [JsonProperty] private string legs = null;
        [JsonProperty] private string hands = null;
        [JsonProperty] private string feet = null;

        [JsonIgnore] public Equipment Accessory => Get(this.accessory);
        [JsonIgnore] public Equipment Head => Get(this.head);
        [JsonIgnore] public Equipment Torso => Get(this.torso);
        [JsonIgnore] public Equipment Legs => Get(this.legs);
        [JsonIgnore] public Equipment Hands => Get(this.hands);
        [JsonIgnore] public Equipment Feet => Get(this.feet);

        private Equipment Get(string id) => string.IsNullOrEmpty(id) ? null : GameCtx.One.GetEquipment(id);

        public void SetEquipped(EquipmentType equipmentType, string equipmentID)
        {
            switch (equipmentType)
            {
                case EquipmentType.Accessory: this.accessory = equipmentID; break;
                case EquipmentType.Head: this.head = equipmentID; break;
                case EquipmentType.Torso: this.torso = equipmentID; break;
                case EquipmentType.Hands: this.hands = equipmentID; break;
                case EquipmentType.Legs: this.legs = equipmentID; break;
                case EquipmentType.Feet: this.feet = equipmentID; break;
                default: Debug.LogError($"[CharacterStats:EquipByType] Tried to equip an equipment without a type"); return;
            }
        }

        public Equipment GetEquipped(EquipmentType equipmentType) => Get(GetEquippedID(equipmentType));

        public string GetEquippedID(EquipmentType equipmentType) => equipmentType switch
        {
            EquipmentType.Accessory => this.accessory,
            EquipmentType.Head => this.head,
            EquipmentType.Torso => this.torso,
            EquipmentType.Hands => this.hands,
            EquipmentType.Legs => this.legs,
            EquipmentType.Feet => this.feet,
            _ => throw new ArgumentException("[EquipmentStats:GetEquippedID] The given EquipmentType was null or invalid"),
        };

        public bool Equipped(EquipmentType equipmentType) => !string.IsNullOrEmpty(GetEquippedID(equipmentType));

        public int SeriesCompletionPercent(string seriesID)
        {
            EquipmentType[] types = Enum.GetValues(typeof(EquipmentType)) as EquipmentType[];
            int total = 0;
            foreach (EquipmentType equipmentType in types)
                if (!string.IsNullOrEmpty(seriesID) && GetEquipped(equipmentType)?.SeriesID == seriesID)
                    total++;
            return total * 100 / (types?.Length ?? 1);
        }
    }
}