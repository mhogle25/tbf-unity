using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using BF2D.Enums;
using BF2D.Game.Enums;
using UnityEngine;

namespace BF2D.Game
{
    [Serializable]
    public class CharacterStats : Entity
    {
        [JsonIgnore] public override string ID { get => this.id; set => this.id = value; }
        [JsonProperty] private string id = string.Empty;

        [JsonIgnore] public string PrefabID => this.prefabID;
        [JsonProperty] private readonly string prefabID = string.Empty;

        //
        // Stats properties and peripheral data
        //
        [JsonIgnore] public int Health => this.health; 
        [JsonIgnore] public int Stamina => this.stamina; 
        [JsonIgnore] public bool Dead => this.health < 1; 
        [JsonProperty] private int health = 0;
        [JsonProperty] private int stamina = 0;

        [JsonIgnore] public int Level => this.CurrentJob?.Level ?? 0; 

        [JsonIgnore] public JobInfo CurrentJob => this.job; 
        [JsonIgnore] public IEnumerable<JobInfo> InactiveJobs => this.inactiveJobs; 
        [JsonProperty] private JobInfo job = null;
        [JsonProperty] private readonly List<JobInfo> inactiveJobs = new();

        // Max health properties
        [JsonIgnore] public int MaxHealth
        {
            get
            {
                int value = this.Constitution + this.MaxHealthModifier;
                if (value < 0) value = 0;
                return value;
            }
        }
        [JsonIgnore] public int MaxHealthModifier => TotalPersistentEffectModifier(CharacterStatsProperty.MaxHealth) + this.CurrentJob?.MaxHealthModifier ?? 0;
        [JsonIgnore] public int Constitution => this.constitution; 
        [JsonProperty] private int constitution = 0;

        // Max stamina properties
        [JsonIgnore] public int MaxStamina
        {
            get
            {
                int value = this.Endurance + this.MaxStaminaModifier;
                if (value < 0) value = 0;
                return value;
            }
        }
        [JsonIgnore] public int MaxStaminaModifier => TotalPersistentEffectModifier(CharacterStatsProperty.MaxStamina) + this.CurrentJob?.MaxStaminaModifier ?? 0;
        [JsonIgnore] private int Endurance => this.endurance; 
        [JsonProperty] private int endurance = 0;

        // Speed properties
        [JsonIgnore] public int Speed 
        { 
            get 
            {
                int value = this.Swiftness + this.SpeedModifier;
                if (value < 0) value = 0;
                return value; 
            } 
        }
        [JsonIgnore] public int SpeedModifier => TotalPersistentEffectModifier(CharacterStatsProperty.Speed) + this.CurrentJob?.SpeedModifier ?? 0;
        [JsonIgnore] public int Swiftness => this.swiftness; 
        [JsonProperty] private int swiftness = 0;

        // Attack properties
        [JsonIgnore] public int Attack 
        { 
            get 
            {
                int value = this.Strength + this.AttackModifier;
                if (value < 0) value = 0;
                return value; 
            } 
        }
        [JsonIgnore] public int AttackModifier => TotalPersistentEffectModifier(CharacterStatsProperty.Attack) + this.CurrentJob?.AttackModifier ?? 0;
        [JsonIgnore] public int Strength => this.strength; 
        [JsonProperty] private int strength = 0;

        // Defense properties
        [JsonIgnore] public int Defense 
        { 
            get 
            {
                int value = this.Toughness + this.DefenseModifier;
                if (value < 0) value = 0;
                return value;
            } 
        }
        [JsonIgnore] public int DefenseModifier => TotalPersistentEffectModifier(CharacterStatsProperty.Defense) + this.CurrentJob?.DefenseModifier ?? 0;
        [JsonIgnore] public int Toughness => this.toughness; 
        [JsonProperty] private int toughness = 0;

        // Focus properties
        [JsonIgnore] public int Focus 
        { 
            get 
            {
                int value = this.Will + this.FocusModifier;
                if (value < 0) value = 0;
                return value;
            } 
        }
        [JsonIgnore] public int FocusModifier => TotalPersistentEffectModifier(CharacterStatsProperty.Focus) + this.CurrentJob?.FocusModifier ?? 0;
        [JsonIgnore] public int Will => this.will; 
        [JsonProperty] private int will = 0;

        // Luck properties
        [JsonIgnore] public int Luck 
        { 
            get 
            { 
                int value = this.Fortune + this.LuckModifier;
                if (value < 0) value = 0;
                return value;
            } 
        }
        [JsonIgnore] public int LuckModifier => TotalPersistentEffectModifier(CharacterStatsProperty.Luck) + this.CurrentJob?.LuckModifier ?? 0;
        [JsonIgnore] public int Fortune => this.fortune; 
        [JsonProperty] private int fortune = 0;

        // Misc
        [JsonIgnore] public bool CritImmune => this.critImmune; 
        [JsonProperty] private bool critImmune = false;

        //
        // Equipped
        //
        [JsonProperty] private readonly EquipmentStats equipped = new();

        [JsonIgnore] public Equipment Head => this.equipped.Head;
        [JsonIgnore] public Equipment Torso => this.equipped.Torso;
        [JsonIgnore] public Equipment Legs => this.equipped.Legs;
        [JsonIgnore] public Equipment Hands => this.equipped.Hands;
        [JsonIgnore] public Equipment Feet => this.equipped.Feet;
        [JsonIgnore] public Equipment Accessory => this.equipped.Accessory;

        //
        // Status effects
        //
        [JsonIgnore] public IEnumerable<StatusEffectInfo> StatusEffects => this.statusEffects; 
        [JsonProperty] private readonly List<StatusEffectInfo> statusEffects = new();

        //
        // Carrying
        //
        [JsonIgnore] public int ItemsCount => this.items.Count; 
        [JsonIgnore] public IItemHolder Items => this.items; 
        [JsonProperty] private readonly ItemHolder items = new();

        [JsonIgnore] public int EquipmentCount => this.equipment.Count;
        [JsonIgnore] public IEquipmentHolder Equipment => this.equipment; 
        [JsonProperty] private readonly EquipmentHolder equipment = new();

        //
        // Combat Properties
        // 
        [JsonIgnore] public Combat.CharacterCombatAI CombatAI => this.combatAI; 
        [JsonProperty] private readonly Combat.CharacterCombatAI combatAI = new();

        [JsonIgnore] public int GridPosition => this.gridPosition; 
        [JsonProperty] private int gridPosition = 0;

        // Loot
        [JsonIgnore] public IEnumerable<EntityLoot> ItemLoot => this.itemLoot; 
        [JsonIgnore] public IEnumerable<EntityLoot> EquipmentLoot => this.equipmentLoot; 
        [JsonIgnore] public IEnumerable<EntityLoot> GemLoot => this.gemLoot; 
        [JsonIgnore] public int CurrencyLoot => this.currencyLoot; 
        [JsonIgnore] public int EtherLoot => this.etherLoot; 
        [JsonProperty] private readonly List<EntityLoot> itemLoot = new();
        [JsonProperty] private readonly List<EntityLoot> equipmentLoot = new();
        [JsonProperty] private readonly List<EntityLoot> gemLoot = new();
        [JsonProperty] private readonly int currencyLoot = 1;
        [JsonProperty] private readonly int etherLoot = 0;

        #region Stats Property Actions
        public int GetStatsProperty(CharacterStatsProperty property) => property switch
        {
            CharacterStatsProperty.Speed => this.Speed,
            CharacterStatsProperty.Attack => this.Attack,
            CharacterStatsProperty.Defense => this.Defense,
            CharacterStatsProperty.Focus => this.Focus,
            CharacterStatsProperty.Luck => this.Luck,
            CharacterStatsProperty.MaxHealth => this.MaxHealth,
            CharacterStatsProperty.MaxStamina => this.MaxStamina,
            _ => throw new ArgumentException("[CharacterStats:GetStatsProperty] The given CharacterStatsProperty was null or invalid")
        };

        public int Damage(int damage)
        {
            int value = (damage - this.Defense) > 0 ? (damage - this.Defense) : 1;
            this.health -= value;
            return value;
        }

        public int DirectDamage(int damage)
        {
            int value = damage > 0 ? damage : 1;
            this.health -= value;
            return value;
        }

        public int CriticalDamage(int damage)
        {
            int value = damage > 0 ? damage * this.CurrentJob.CritMultiplier : 1;
            this.health -= value;
            return value;
        }

        public int PsychicDamage(int damage)
        {
            int value = (damage - this.Focus) > 0 ? (damage - this.Focus) : 1;
            this.health -= value;
            return value;
        }

        public int Heal(int healing)
        {
            int value = healing > 0 ? healing : 1;

            if (this.health + value > this.MaxHealth)
                return ResetHealth();

            this.health += value;
            return value;
        }

        public int Recover(int recovery)
        {
            int value = recovery > 0 ? recovery : 1;

            if (this.stamina + value > this.MaxStamina)
                return ResetStamina();

            this.stamina += value;
            return value;
        }

        public int Exert(int exertion)
        {
            int value = exertion > 0 ? exertion : 1;
            this.stamina -= value;
            return value;
        }

        /// <returns>The amount of health restored</returns>
        public int ResetHealth()
        {
            int healthBefore = this.Health;
            this.health = this.MaxHealth;
            return this.Health - healthBefore;
        }

        /// <returns>The amount of stamina restored</returns>
        public int ResetStamina()
        {
            int staminaBefore = this.Stamina;
            this.stamina = this.MaxStamina;
            return this.Stamina - staminaBefore;
        }

        public int ConstitutionUp(int amount)
        {
            int value = amount > 0 ? amount : 1;
            this.constitution += value;
            return value;
        }

        public int EnduranceUp(int amount)
        {
            int value = amount > 0 ? amount : 1;
            this.endurance += value;
            return value;
        }

        public int SwiftnessUp(int amount)
        {
            int value = amount > 0 ? amount : 1;
            this.swiftness += value;
            return value;
        }

        public int StrengthUp(int amount)
        {
            int value = amount > 0 ? amount : 1;
            this.strength += value;
            return value;
        }

        public int ToughnessUp(int amount)
        {
            int value = amount > 0 ? amount : 1;
            this.toughness += value;
            return value;
        }

        public int WillUp(int amount)
        {
            int value = amount > 0 ? amount : 1;
            this.will += value;
            return value;
        }

        public int FortuneUp(int amount)
        {
            int value = amount > 0 ? amount : 1;
            this.fortune += value;
            return value;
        }
        #endregion

        #region Generic Public Utilities
        public string GetStatsPropertyText(CharacterStatsProperty property)
        {
            return $"{GetStatsProperty(property)}{Strings.Character.GetStatsPropertySymbol(property)}";
        }

        public string GetModifierText(CharacterStatsProperty property)
        {
            if (GetStatsProperty(property) < 1)
                return string.Empty;

            return $"+{GetStatsPropertyText(property)}";
        }

        public JobInfo.LevelUpInfo GrantExperience(long experience)
        {
            return this.CurrentJob.GrantExperience(this, experience);
        }
        #endregion

        #region Equipment Actions
        public void Equip(EquipmentInfo info)
        {
            Equipment equipment = info.Get();

            if (equipment is null)
            {
                Debug.LogWarning($"[CharacterStats:Equip] Tried to equip to {this.Name} but the equipment given was null");
                return;
            }

            string id = this.Equipment.Extract(info);
            this.equipped.SetEquipped(equipment.Type, id);
        }

        public void Unequip(EquipmentType equipmentType)
        {
            string id = this.equipped.GetEquippedID(equipmentType);
            this.equipped.SetEquipped(equipmentType, null);
            this.Equipment.Acquire(id);
        }

        public bool Equipped(EquipmentType equipmentType) => this.equipped.Equipped(equipmentType);

        public Equipment GetEquipped(EquipmentType equipmentType) => this.equipped.GetEquipped(equipmentType);
        public string GetEquippedID(EquipmentType equipmentType) => this.equipped.GetEquippedID(equipmentType);

        private int TotalEquipmentModifier(CharacterStatsProperty property)
        {
            int total = 0;
            foreach (EquipmentType equipmentType in Enum.GetValues(typeof(EquipmentType)))
            {
                Equipment equipment = GetEquipped(equipmentType);
                total += equipment?.GetModifier(property) ?? 0;
            }
            return total;
        }
        #endregion

        #region Status Effect Actions
        public bool ApplyStatusEffect(string id)
        {
            StatusEffectInfo info = AddStatusEffect(id);

            if (info is null)
                return false;

            return true;
        }

        public void RemoveStatusEffect(StatusEffectInfo info)
        {
            if (info is null)
            {
                Debug.LogError($"[CharacterStats:RemoveStatusEffect] Tried to remove a status effect from {this.Name} but the status effect info given was null");
                return;
            }

            this.statusEffects.Remove(info);
        }

        private StatusEffectInfo AddStatusEffect(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogError($"[CharacterStats:AddStatusEffect] Tried to apply a status effect to {this.Name} but the status effect id given was invalid");
                return null;
            }

            StatusEffectInfo newInfo = new(id);
            if (newInfo.Get().Singleton && this.statusEffects.Exists(info => info.ID == id))
                return null;

            this.statusEffects.Add(newInfo);
            return newInfo;
        }

        private int TotalStatusEffectModifier(CharacterStatsProperty property)
        {
            int total = 0;
            foreach (StatusEffectInfo info in this.StatusEffects)
            {
                StatusEffect statusEffect = info.Get();
                total += statusEffect.GetModifier(property);
            }
            return total;
        }
        #endregion

        #region Private Utilities
        private int TotalPersistentEffectModifier(CharacterStatsProperty property) => TotalEquipmentModifier(property) + TotalStatusEffectModifier(property);
        #endregion
    }
}
