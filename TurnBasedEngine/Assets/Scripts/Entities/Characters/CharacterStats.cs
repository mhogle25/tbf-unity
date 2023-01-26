using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using BF2D.Enums;
using UnityEngine;
using BF2D.Game.Enums;

namespace BF2D.Game
{
    [Serializable]
    public class CharacterStats : Entity
    {
        [JsonIgnore] public string PrefabID { get { return this.prefabID; } }
        [JsonProperty] private readonly string prefabID = string.Empty;

        public class StatModifier
        {
            public CharacterStatsProperty Property { get { return this.property; } set { this.property = value; } }
            private CharacterStatsProperty property = CharacterStatsProperty.Speed;

            public int Equipment { get { return this.equipment; } }
            private int equipment = 0;
            public int StatusEffect { get { return this.statusEffect; } }
            private int statusEffect = 0;

            public void ApplyStatusEffect(StatusEffect statusEffect)
            {
                this.statusEffect += statusEffect.GetModifier(this.property);
            }

            public void RemoveStatusEffect(StatusEffect statusEffect)
            {
                this.statusEffect -= statusEffect.GetModifier(this.property);
            }

            public void Equip(Equipment equipment)
            {
                this.equipment += equipment.GetModifier(this.property);
            }

            public void Unequip(Equipment equipment)
            {
                this.equipment -= equipment.GetModifier(this.property);
            }
        }
        [JsonIgnore] public bool Dead { get { return this.health < 1; } }
        [JsonIgnore] public int Health { get { return this.health; } }
        [JsonProperty] private int health = 0;
        [JsonIgnore] public int MaxHealth { get { return this.maxHealth; } }
        [JsonIgnore] private int maxHealth = 0;
        [JsonIgnore] public int Stamina { get { return this.stamina; } }
        [JsonProperty] private int stamina = 0;
        [JsonIgnore] public int MaxStamina { get { return this.maxStamina; } }
        [JsonIgnore] private int maxStamina = 0;

        [JsonIgnore] public uint Speed 
        { 
            get 
            {
                int value = this.speedModifier.Equipment + this.speedModifier.StatusEffect + this.swiftness;
                if (value < 0) value = 0;
                return (uint)value; 
            } 
        }
        [JsonIgnore] public StatModifier SpeedModifier { get { return this.speedModifier; }}
        [JsonIgnore] private readonly StatModifier speedModifier = new() { Property = CharacterStatsProperty.Speed };
        [JsonIgnore] public int Swiftness { get { return this.swiftness; } }
        [JsonProperty] private int swiftness = 0;

        [JsonIgnore] public uint Attack 
        { 
            get 
            {
                int value = this.attackModifier.Equipment + this.attackModifier.StatusEffect + this.strength;
                if (value < 0) value = 0;
                return (uint)value; 
            } 
        }
        [JsonIgnore] public StatModifier AttackModifier { get { return this.attackModifier; } }
        [JsonIgnore] private StatModifier attackModifier = new() { Property = CharacterStatsProperty.Attack };
        [JsonIgnore] public int Strength { get { return this.strength; } }
        [JsonProperty] private int strength = 0;

        [JsonIgnore] public uint Defense 
        { 
            get 
            {
                int value = this.defenseModifier.Equipment + this.defenseModifier.StatusEffect + this.toughness;
                if (value < 0) value = 0;
                return (uint)value;
            } 
        }
        [JsonIgnore] public StatModifier DefenseModifier { get { return this.defenseModifier; }}
        [JsonIgnore] private readonly StatModifier defenseModifier = new() { Property = CharacterStatsProperty.Defense };
        [JsonIgnore] public int Toughness { get { return this.toughness; } }
        [JsonProperty] private int toughness = 0;

        [JsonIgnore] public uint Focus 
        { 
            get 
            {
                int value = this.focusModifier.Equipment + this.focusModifier.StatusEffect + this.will;
                if (value < 0) value = 0;
                return (uint)value;
            } 
        }
        [JsonIgnore] public StatModifier FocusModifier { get { return this.focusModifier; } }
        [JsonIgnore] private readonly StatModifier focusModifier = new() { Property = CharacterStatsProperty.Focus };
        [JsonIgnore] public int Will { get { return this.will; } }
        [JsonProperty] private int will = 0;

        [JsonIgnore] public uint Luck 
        { 
            get 
            { 
                int value = this.luckModifier.Equipment + this.luckModifier.StatusEffect + this.fortune;
                if (value < 0) value = 0;
                return (uint)value;
            } 
        }
        [JsonIgnore] public StatModifier LuckModifier { get { return this.luckModifier; } }
        [JsonIgnore] private readonly StatModifier luckModifier = new() { Property = CharacterStatsProperty.Luck };
        [JsonIgnore] public int Fortune { get { return this.fortune; } }
        [JsonProperty] private int fortune = 0;
        [JsonIgnore] public int Experience { get { return this.experience; } }
        [JsonProperty] private int experience = 0;
        [JsonIgnore] public int Level { get { return this.level; } }
        [JsonProperty] private int level = 1;

        [JsonIgnore] public string Head { get { return this.head; } }
        [JsonProperty] private string head = string.Empty;
        [JsonIgnore] public string Torso { get { return this.torso; } }
        [JsonProperty] private string torso = string.Empty;
        [JsonIgnore] public string Legs { get { return this.legs; } }
        [JsonProperty] private string legs = string.Empty;
        [JsonIgnore] public string Hands { get { return this.hands; } }
        [JsonProperty] private string hands = string.Empty;
        [JsonIgnore] public string Feet { get { return this.feet; } }
        [JsonProperty] private string feet = string.Empty;
        [JsonIgnore] public string Accessory { get { return this.accessory; } }
        [JsonProperty] private string accessory = string.Empty;

        [JsonIgnore] public IEnumerable<StatusEffect> StatusEffects { get { return this.statusEffects; } }
        [JsonProperty] private readonly List<StatusEffect> statusEffects = new();

        [JsonIgnore] public int GridPosition { get { return this.gridPosition; } }
        [JsonProperty] private int gridPosition = 0;

        [JsonIgnore] public IEnumerable<ItemInfo> Items { get { return this.items; } }
        [JsonProperty] private readonly List<ItemInfo> items = new();
        [JsonIgnore] public IEnumerable<EquipmentInfo> Equipments { get { return this.equipments; } }
        [JsonProperty] private readonly List<EquipmentInfo> equipments = new();

        public CharacterStats Setup()
        {
            foreach (EquipmentType equipmentType in Enum.GetValues(typeof(EquipmentType)))
            {
                EquipModifierUpdate(GetEquipped(equipmentType));
            }

            foreach (StatusEffect statusEffect in this.statusEffects)
            {
                ApplyStatusEffectModifierUpdate(statusEffect);
            }

            return this;
        }

        public void Damage(int damage)
        {
            this.health -= (damage - (int)Defense) > 0 ? (damage - (int)Defense) : 1;
        }

        public void DirectDamage(int damage)
        {
            this.health -= damage > 0 ? damage : 1;
        }

        public void CriticalDamage(int damage)
        {
            this.health -= damage > 0 ? damage * CritMultiplier() : 1;
        }

        public void PsychicDamage(int damage)
        {
            this.health -= (damage - (int)Focus) > 0 ? (damage - (int)Focus) : 1;
        }

        public void Heal(int healing)
        {
            int value = healing > 0 ? healing : 1;

            if (this.health + value > this.maxHealth)
            {
                this.health = this.maxHealth;
                return;
            }

            this.health += value;
        }

        public void Recover(int recovery)
        {
            int value = recovery > 0 ? recovery : 1;

            if (this.stamina + value > this.maxStamina)
            {
                this.stamina = this.maxStamina;
                return;
            }

            this.stamina += value;
        }

        public void Exert(int exertion)
        {
            this.stamina -= exertion > 0 ? exertion : 1;
        }

        public void ResetHealth()
        {
            this.health = this.maxHealth;
        }

        public void ResetStamina()
        {
            this.stamina = this.maxStamina;
        }

        public void Equip(string id)
        {
            Equip(GameInfo.Instance.GetEquipment(id), id);
        }

        public void Unequip(EquipmentType equipmentType)
        {
            string id = GetEquipped(equipmentType);
            UnequipModifierUpdate(id);
            EquipByType(equipmentType, null);
        }

        public void ApplyStatusEffect(StatusEffect statusEffect)
        {
            if (statusEffect is null)
            {
                Debug.LogWarning($"[CharacterStats:ApplyStatusEffect] Tried to apply a status effect to {this.name} but the status effect given was null");
                return;
            }

            ApplyStatusEffectModifierUpdate(statusEffect);
            this.statusEffects.Add(statusEffect);
        }

        public void RemoveStatusEffect(StatusEffect statusEffect)
        {
            if (statusEffect is null)
            {
                Debug.LogWarning($"[CharacterStats:RemoveStatusEffect] Tried to apply a status effect to {this.name} but the status effect given was null");
                return;
            }

            RemoveStatusEffectModifierUpdate(statusEffect);
            this.statusEffects.Remove(statusEffect);
        }

        public uint GetStatsProperty(CharacterStatsProperty property)
        {
            return property switch
            {
                CharacterStatsProperty.Speed => this.Speed,
                CharacterStatsProperty.Attack => this.Attack,
                CharacterStatsProperty.Defense => this.Defense,
                CharacterStatsProperty.Focus => this.Focus,
                CharacterStatsProperty.Luck => this.Luck,
                _ => 0
            };
        }

        public string GetEquipped(EquipmentType equipmentType)
        {
            return equipmentType switch
            {
                EquipmentType.Accessory => this.Accessory,
                EquipmentType.Head => this.Head,
                EquipmentType.Torso => this.Torso,
                EquipmentType.Hands => this.Hands,
                EquipmentType.Legs => this.Legs,
                EquipmentType.Feet => this.Feet,
                _ => null,
            };
        }

        public void SetName(string newName)
        {
            this.name = newName;
        }

        private void Equip(Equipment equipment, string id)
        {
            if (equipment is null)
            {
                Debug.LogWarning($"[CharacterStats:Equip] Tried to equip to {this.name} but the equipment given was null");
                return;
            }
            EquipModifierUpdate(id);
            EquipByType(equipment.Type, id);
        }

        private void EquipByType(EquipmentType equipmentType, string equipmentID)
        {
            switch (equipmentType)
            {
                case EquipmentType.Accessory: this.accessory = equipmentID; break;
                case EquipmentType.Head: this.head = equipmentID; break;
                case EquipmentType.Torso: this.torso = equipmentID; break;
                case EquipmentType.Hands: this.hands = equipmentID; break;
                case EquipmentType.Legs: this.legs = equipmentID; break;
                case EquipmentType.Feet: this.feet = equipmentID; break;
                default: Debug.LogError($"[CharacterStats:SetEquipmentByType] Tried to equip an equipment that didn't have a type to {this.name}"); return;
            }
        }

        private void EquipModifierUpdate(string equipmentID)
        {
            if (equipmentID == string.Empty || equipmentID is null)
                return;

            Equipment equipment = GameInfo.Instance.GetEquipment(equipmentID);

            if (equipment == null)
                return;
            this.speedModifier.Equip(equipment);
            this.attackModifier.Equip(equipment);
            this.defenseModifier.Equip(equipment);
            this.focusModifier.Equip(equipment);
            this.luckModifier.Equip(equipment);
        }

        private void UnequipModifierUpdate(string equipmentID)
        {
            if (equipmentID == string.Empty || equipmentID is null)
                return;

            Equipment equipment = GameInfo.Instance.GetEquipment(equipmentID);

            if (equipment == null)
                return;
            this.speedModifier.Unequip(equipment);
            this.attackModifier.Unequip(equipment);
            this.defenseModifier.Unequip(equipment);
            this.focusModifier.Unequip(equipment);
            this.luckModifier.Unequip(equipment);
        }

        private void ApplyStatusEffectModifierUpdate(StatusEffect statusEffect)
        {
            if (statusEffect == null)
                return;
            this.speedModifier.ApplyStatusEffect(statusEffect);
            this.attackModifier.ApplyStatusEffect(statusEffect);
            this.defenseModifier.ApplyStatusEffect(statusEffect);
            this.focusModifier.ApplyStatusEffect(statusEffect);
            this.luckModifier.ApplyStatusEffect(statusEffect);
        }

        private void RemoveStatusEffectModifierUpdate(StatusEffect statusEffect)
        {
            if (statusEffect == null)
                return;
            this.speedModifier.RemoveStatusEffect(statusEffect);
            this.attackModifier.RemoveStatusEffect(statusEffect);
            this.defenseModifier.RemoveStatusEffect(statusEffect);
            this.focusModifier.RemoveStatusEffect(statusEffect);
            this.luckModifier.RemoveStatusEffect(statusEffect);
        }

        private int CritMultiplier()
        {
            return 2;   //
        }
    }
}
