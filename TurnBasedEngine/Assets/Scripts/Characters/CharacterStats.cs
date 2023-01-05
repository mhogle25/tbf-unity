using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using BF2D.Enums;
using UnityEngine;

namespace BF2D.Game
{
    [Serializable]
    public class CharacterStats
    {
        public class StatModifier
        {
            public int Equipment { get { return this.equipment; } }
            private int equipment = 0;
            public int StatusEffect { get { return this.statusEffect; } }
            private int statusEffect = 0;

            public void ApplyStatusEffect(StatusEffect statusEffect)
            {

            }

            public void RemoveStatusEffect(StatusEffect statusEffect)
            {

            }

            public void Equip(Equipment equipment)
            {

            }

            public void Unequip(Equipment equipment)
            {

            }
        }

        [JsonIgnore] public string ID { get { return this.id; } }
        [JsonProperty] private string id = string.Empty;
        [JsonIgnore] public string Name { get { return this.name; } set { this.name = value; } }
        [JsonProperty] private string name = string.Empty;
        [JsonIgnore] public string Description { get { return this.description; } set { this.description = value; } }
        [JsonProperty] private string description = string.Empty;
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
        [JsonIgnore] private readonly StatModifier speedModifier = new();
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
        [JsonIgnore] private     StatModifier attackModifier = new();
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
        [JsonIgnore] private readonly StatModifier defenseModifier = new();
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
        [JsonIgnore] private readonly StatModifier focusModifier = new();
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
        [JsonIgnore] private readonly StatModifier luckModifier = new();
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

        [JsonIgnore] public int GridPosition { get { return this.gridPosition; } }
        [JsonProperty] private int gridPosition = 0;

        [JsonIgnore] public List<ItemInfo> Items { get { return this.items; } }
        [JsonProperty] private readonly List<ItemInfo> items = new();
        [JsonIgnore] public List<EquipmentInfo> Equipments { get { return this.equipments; } }
        [JsonProperty] private readonly List<EquipmentInfo> equipments = new();
        [JsonIgnore] public List<string> StatusEffects { get { return this.statusEffects; } }
        [JsonProperty] private readonly List<string> statusEffects = new();


        public void Damage(int damage)
        {
            this.health -= (damage - (int)Defense) > 0 ? (damage - (int)Defense) : 1;
        }

        public void CriticalDamage(int damage)
        {
            this.health -= damage > 0 ? damage : 1;
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

        public void Equip(Equipment equipment)
        {
            if (equipment is null)
            {
                Debug.LogWarning($"[CharacterStats] Tried to equip to {this.name} but the equipment given was null");
                return;
            }

            SpeedModifier.Equip(equipment);
            AttackModifier.Equip(equipment);
            DefenseModifier.Equip(equipment);
            FocusModifier.Equip(equipment);
            LuckModifier.Equip(equipment);
        }

        public void Unequip(Equipment equipment)
        {
            if (equipment is null)
            {
                Debug.LogWarning($"[CharacterStats] Tried to equip to {this.name} but the equipment given was null");
                return;
            }

            SpeedModifier.Unequip(equipment);
            AttackModifier.Unequip(equipment);
            DefenseModifier.Unequip(equipment);
            FocusModifier.Unequip(equipment);
            LuckModifier.Unequip(equipment);
        }

        public void ApplyStatusEffect(StatusEffect statusEffect)
        {
            if (statusEffect is null)
            {
                Debug.LogWarning($"[CharacterStats] Tried to apply a status effect to {this.name} but the status effect given was null");
                return;
            }

            SpeedModifier.ApplyStatusEffect(statusEffect);
            AttackModifier.ApplyStatusEffect(statusEffect);
            DefenseModifier.ApplyStatusEffect(statusEffect);
            FocusModifier.ApplyStatusEffect(statusEffect);
            LuckModifier.ApplyStatusEffect(statusEffect);
        }

        public void RemoveStatusEffect(StatusEffect statusEffect)
        {
            if (statusEffect is null)
            {
                Debug.LogWarning($"[CharacterStats] Tried to apply a status effect to {this.name} but the status effect given was null");
                return;
            }

            SpeedModifier.RemoveStatusEffect(statusEffect);
            AttackModifier.RemoveStatusEffect(statusEffect);
            DefenseModifier.RemoveStatusEffect(statusEffect);
            FocusModifier.RemoveStatusEffect(statusEffect);
            LuckModifier.RemoveStatusEffect(statusEffect);
        }

        public uint GetStatsProperty(CharacterStatsProperty property)
        {
            return property switch
            {
                CharacterStatsProperty.None => 0,
                CharacterStatsProperty.Speed => this.Speed,
                CharacterStatsProperty.Attack => this.Attack,
                CharacterStatsProperty.Defense => this.Defense,
                CharacterStatsProperty.Focus => this.Focus,
                CharacterStatsProperty.Luck => this.Luck,
                _ => throw new ArgumentException("[CharacterStats] The given property was null or invalid")
            };
        }
    }
}
