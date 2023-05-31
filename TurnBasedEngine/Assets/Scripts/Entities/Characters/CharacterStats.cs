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

        [JsonIgnore] public string PrefabID { get => this.prefabID; }
        [JsonProperty] private readonly string prefabID = string.Empty;

        //
        // Stats properties and peripheral data
        //
        [JsonIgnore] public int Health { get => this.health; }
        [JsonIgnore] public int Stamina { get => this.stamina; }
        [JsonIgnore] public bool Dead { get => this.health < 1; }
        [JsonProperty] private int health = 0;
        [JsonProperty] private int stamina = 0;

        [JsonIgnore] public int Level { get => this.CurrentJob is null ? 0 : this.CurrentJob.Level; }

        [JsonIgnore] public JobInfo CurrentJob { get => this.job; }
        [JsonIgnore] public IEnumerable<JobInfo> InactiveJobs { get => this.inactiveJobs; }
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
        [JsonIgnore] public int MaxHealthModifier
        {
            get
            {
                return
                    TotalEquipmentModifier(CharacterStatsProperty.MaxHealth) +
                    TotalStatusEffectModifier(CharacterStatsProperty.MaxHealth) +
                    (this.CurrentJob is null ? 0 : this.CurrentJob.MaxHealthModifier)
                ;
            }
        }
        [JsonIgnore] public int Constitution { get => this.constitution; }
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
        [JsonIgnore] public int MaxStaminaModifier
        {
            get
            {
                return
                    TotalEquipmentModifier(CharacterStatsProperty.MaxStamina) +
                    TotalStatusEffectModifier(CharacterStatsProperty.MaxStamina) +
                    (this.CurrentJob is null ? 0 : this.CurrentJob.MaxStaminaModifier)
                ;
            }
        }
        [JsonIgnore] private int Endurance { get => this.endurance; }
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
        [JsonIgnore] public int SpeedModifier
        {
            get
            {
                return
                    TotalEquipmentModifier(CharacterStatsProperty.Speed) +
                    TotalStatusEffectModifier(CharacterStatsProperty.Speed) +
                    (this.CurrentJob is null ? 0 : this.CurrentJob.SpeedModifier)
                ;
            }
        }
        [JsonIgnore] public int Swiftness { get => this.swiftness; }
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
        [JsonIgnore]
        public int AttackModifier
        {
            get
            {
                return
                    TotalEquipmentModifier(CharacterStatsProperty.Attack) +
                    TotalStatusEffectModifier(CharacterStatsProperty.Attack) +
                    (this.CurrentJob is null ? 0 : this.CurrentJob.AttackModifier)
                ;
            }
        }
        [JsonIgnore] public int Strength { get => this.strength; }
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
        [JsonIgnore]
        public int DefenseModifier
        {
            get
            {
                return
                    TotalEquipmentModifier(CharacterStatsProperty.Defense) +
                    TotalStatusEffectModifier(CharacterStatsProperty.Defense) +
                    (this.CurrentJob is null ? 0 : this.CurrentJob.DefenseModifier)
                ;
            }
        }
        [JsonIgnore] public int Toughness { get => this.toughness; }
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
        [JsonIgnore]
        public int FocusModifier
        {
            get
            {
                return
                    TotalEquipmentModifier(CharacterStatsProperty.Focus) +
                    TotalStatusEffectModifier(CharacterStatsProperty.Focus) +
                    (this.CurrentJob is null ? 0 : this.CurrentJob.FocusModifier)
                ;
            }
        }
        [JsonIgnore] public int Will { get => this.will; }
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
        [JsonIgnore]
        public int LuckModifier
        {
            get
            {
                return
                    TotalEquipmentModifier(CharacterStatsProperty.Luck) +
                    TotalStatusEffectModifier(CharacterStatsProperty.Luck) +
                    (this.CurrentJob is null ? 0 : this.CurrentJob.LuckModifier)
                ;
            }
        }
        [JsonIgnore] public int Fortune { get => this.fortune; }
        [JsonProperty] private int fortune = 0;

        // Misc
        [JsonIgnore] public bool CritImmune { get => this.critImmune; }
        [JsonProperty] private bool critImmune = false;

        //
        // Equipment Properties
        //
        [JsonIgnore] public Equipment Head { get => string.IsNullOrEmpty(this.head) ? null : GameInfo.Instance.GetEquipment(this.head); }
        [JsonIgnore] public Equipment Torso { get => string.IsNullOrEmpty(this.torso) ? null : GameInfo.Instance.GetEquipment(this.torso); }
        [JsonIgnore] public Equipment Legs { get => string.IsNullOrEmpty(this.legs) ? null : GameInfo.Instance.GetEquipment(this.legs); }
        [JsonIgnore] public Equipment Hands { get => string.IsNullOrEmpty(this.hands) ? null : GameInfo.Instance.GetEquipment(this.hands); }
        [JsonIgnore] public Equipment Feet { get => string.IsNullOrEmpty(this.feet) ? null : GameInfo.Instance.GetEquipment(this.feet); }
        [JsonIgnore] public Equipment Accessory { get => string.IsNullOrEmpty(this.accessory) ? null : GameInfo.Instance.GetEquipment(this.accessory); }
        [JsonProperty] private string head = null;
        [JsonProperty] private string torso = null;
        [JsonProperty] private string legs = null;
        [JsonProperty] private string hands = null;
        [JsonProperty] private string feet = null;
        [JsonProperty] private string accessory = null;

        //
        // Active status effects
        //
        [JsonIgnore] public IEnumerable<StatusEffectInfo> StatusEffects { get => this.statusEffects; }
        [JsonProperty] private readonly List<StatusEffectInfo> statusEffects = new();

        //
        // Carrying
        //
        [JsonIgnore] public int ItemsCount { get => this.items.Count; }
        [JsonIgnore] public IItemHolder Items { get => this.items; }
        [JsonProperty] private readonly ItemHolder items = new();

        [JsonIgnore] public int EquipmentsCount { get => this.equipments.Count; }
        [JsonIgnore] public IEquipmentHolder Equipments { get => this.equipments; }
        [JsonProperty] private readonly EquipmentHolder equipments = new();

        //
        // Combat Properties
        // 
        [JsonIgnore] public Combat.CharacterCombatAI CombatAI { get => this.combatAI; }
        [JsonProperty] private readonly Combat.CharacterCombatAI combatAI = new();

        [JsonIgnore] public int GridPosition { get => this.gridPosition; }
        [JsonProperty] private int gridPosition = 0;

        [JsonIgnore] public IEnumerable<EntityLoot> ItemsLoot { get => this.itemsLoot; }
        [JsonIgnore] public IEnumerable<EntityLoot> EquipmentsLoot { get => this.equipmentsLoot; }
        [JsonIgnore] public int CurrencyLoot { get => this.currencyLoot; }
        [JsonIgnore] public int EtherLoot { get => this.etherLoot; }
        [JsonProperty] private readonly List<EntityLoot> itemsLoot = new();
        [JsonProperty] private readonly List<EntityLoot> equipmentsLoot = new();
        [JsonProperty] private readonly int currencyLoot = 1;
        [JsonProperty] private readonly int etherLoot = 0;

        #region Stats Property Actions
        public int GetStatsProperty(CharacterStatsProperty property)
        {
            return property switch
            {
                CharacterStatsProperty.Speed => this.Speed,
                CharacterStatsProperty.Attack => this.Attack,
                CharacterStatsProperty.Defense => this.Defense,
                CharacterStatsProperty.Focus => this.Focus,
                CharacterStatsProperty.Luck => this.Luck,
                CharacterStatsProperty.MaxHealth => this.MaxHealth,
                CharacterStatsProperty.MaxStamina => this.MaxStamina,
                _ => 0
            };
        }

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
            {
                return ResetHealth();
            }

            this.health += value;
            return value;
        }

        public int Recover(int recovery)
        {
            int value = recovery > 0 ? recovery : 1;

            if (this.stamina + value > this.MaxStamina)
            {
                return ResetStamina();
            }

            this.stamina += value;
            return value;
        }

        public int Exert(int exertion)
        {
            int value = exertion > 0 ? exertion : 1;
            this.stamina -= value;
            return value;
        }

        public int ResetHealth()
        {
            int healthBefore = this.Health;
            this.health = this.MaxHealth;
            return this.Health - healthBefore;
        }

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
            return $"{GetStatsProperty(property)}{Strings.CharacterStats.GetStatsPropertySymbol(property)}";
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

            info.Decrement(this.Equipments);
            EquipByType(equipment.Type, info.ID);
        }

        public void Unequip(EquipmentType equipmentType)
        {
            string id = GetEquippedID(equipmentType);
            EquipByType(equipmentType, null);
            this.Equipments.AcquireEquipment(id);
        }

        public bool IsEquipped(EquipmentType equipmentType)
        {
            return !string.IsNullOrEmpty(GetEquippedID(equipmentType));
        }

        public Equipment GetEquipped(EquipmentType equipmentType)
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

        private string GetEquippedID(EquipmentType equipmentType)
        {
            return equipmentType switch
            {
                EquipmentType.Accessory => this.accessory,
                EquipmentType.Head => this.head,
                EquipmentType.Torso => this.torso,
                EquipmentType.Hands => this.hands,
                EquipmentType.Legs => this.legs,
                EquipmentType.Feet => this.feet,
                _ => null,
            };
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
                default: Debug.LogError($"[CharacterStats:EquipByType] Tried to equip an equipment that didn't have a type to {this.Name}"); return;
            }
        }

        private int TotalEquipmentModifier(CharacterStatsProperty property)
        {
            int total = 0;
            foreach (EquipmentType equipmentType in Enum.GetValues(typeof(EquipmentType)))
            {
                Equipment equipment = GetEquipped(equipmentType);
                total += equipment is not null ? equipment.GetModifier(property) : 0;
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
    }
}
