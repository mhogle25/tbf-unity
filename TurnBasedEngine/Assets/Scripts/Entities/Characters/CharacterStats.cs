using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using BF2D.Enums;
using UnityEngine;
using BF2D.Game.Enums;
using UnityEngine.TextCore.Text;

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

            public int Total { get { return this.Equipment + this.StatusEffect; } }

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

        [JsonIgnore] public int Level { get { return (this.CurrentJob is null ? 0 : this.CurrentJob.Level); } }
        
        [JsonIgnore] public bool Dead { get { return this.health < 1; } }
        [JsonIgnore] public int Health { get { return this.health; } }
        [JsonProperty] private int health = 0;

        [JsonIgnore] public int Stamina { get { return this.stamina; } }
        [JsonProperty] private int stamina = 0;

        [JsonIgnore] public int MaxHealth
        {
            get
            {
                int value = this.Constitution + (this.CurrentJob is null ? 0 : this.CurrentJob.MaxHealthModifier) + this.MaxHealthModifier.Total;
                if (value < 0) value = 0;
                return value;
            }
        }
        [JsonIgnore] public StatModifier MaxHealthModifier { get { return this.maxHealthModifier; } }
        [JsonIgnore] private readonly StatModifier maxHealthModifier = new() { Property = CharacterStatsProperty.MaxHealth };
        [JsonIgnore] public int Constitution { get { return this.constitution; } }
        [JsonProperty] private int constitution = 0;

        [JsonIgnore] public int MaxStamina
        {
            get
            {
                int value = this.Endurance + (this.CurrentJob is null ? 0 : this.CurrentJob.MaxStaminaModifier) + this.MaxStaminaModifier.Total;
                if (value < 0) value = 0;
                return value;
            }
        }
        [JsonIgnore] public StatModifier MaxStaminaModifier { get { return this.maxStaminaModifier; } }
        [JsonIgnore] private readonly StatModifier maxStaminaModifier = new() { Property = CharacterStatsProperty.MaxStamina };
        [JsonIgnore] private int Endurance { get { return this.endurance; } }
        [JsonProperty] private int endurance = 0;

        [JsonIgnore] public int Speed 
        { 
            get 
            {
                int value = this.speedModifier.Total + (this.CurrentJob is null ? 0 : this.CurrentJob.SpeedModifier) + this.swiftness;
                if (value < 0) value = 0;
                return value; 
            } 
        }
        [JsonIgnore] public StatModifier SpeedModifier { get { return this.speedModifier; }}
        [JsonIgnore] private readonly StatModifier speedModifier = new() { Property = CharacterStatsProperty.Speed };
        [JsonIgnore] public int Swiftness { get { return this.swiftness; } }
        [JsonProperty] private int swiftness = 0;

        [JsonIgnore] public int Attack 
        { 
            get 
            {
                int value = this.attackModifier.Total + (this.CurrentJob is null ? 0 : this.CurrentJob.AttackModifier) + this.strength;
                if (value < 0) value = 0;
                return value; 
            } 
        }
        [JsonIgnore] public StatModifier AttackModifier { get { return this.attackModifier; } }
        [JsonIgnore] private StatModifier attackModifier = new() { Property = CharacterStatsProperty.Attack };
        [JsonIgnore] public int Strength { get { return this.strength; } }
        [JsonProperty] private int strength = 0;

        [JsonIgnore] public int Defense 
        { 
            get 
            {
                int value = this.defenseModifier.Total + (this.CurrentJob is null ? 0 : this.CurrentJob.DefenseModifier) + this.toughness;
                if (value < 0) value = 0;
                return value;
            } 
        }
        [JsonIgnore] public StatModifier DefenseModifier { get { return this.defenseModifier; }}
        [JsonIgnore] private readonly StatModifier defenseModifier = new() { Property = CharacterStatsProperty.Defense };
        [JsonIgnore] public int Toughness { get { return this.toughness; } }
        [JsonProperty] private int toughness = 0;

        [JsonIgnore] public int Focus 
        { 
            get 
            {
                int value = this.focusModifier.Total + (this.CurrentJob is null ? 0 : this.CurrentJob.FocusModifier) + this.will;
                if (value < 0) value = 0;
                return value;
            } 
        }
        [JsonIgnore] public StatModifier FocusModifier { get { return this.focusModifier; } }
        [JsonIgnore] private readonly StatModifier focusModifier = new() { Property = CharacterStatsProperty.Focus };
        [JsonIgnore] public int Will { get { return this.will; } }
        [JsonProperty] private int will = 0;

        [JsonIgnore] public int Luck 
        { 
            get 
            { 
                int value = this.luckModifier.Total + (this.CurrentJob is null ? 0 : this.CurrentJob.LuckModifier) + this.fortune;
                if (value < 0) value = 0;
                return value;
            } 
        }
        [JsonIgnore] public StatModifier LuckModifier { get { return this.luckModifier; } }
        [JsonIgnore] private readonly StatModifier luckModifier = new() { Property = CharacterStatsProperty.Luck };
        [JsonIgnore] public int Fortune { get { return this.fortune; } }
        [JsonProperty] private int fortune = 0;

        [JsonIgnore] public bool CritImmune { get { return this.critImmune; } }
        [JsonProperty] private bool critImmune = false;

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

        [JsonIgnore] public int GridPosition { get { return this.gridPosition; } }
        [JsonProperty] private int gridPosition = 0;

        [JsonIgnore] public JobInfo CurrentJob { get { return this.job; } }
        [JsonProperty] private JobInfo job = null;
        [JsonIgnore] public IEnumerable<JobInfo> InactiveJobs { get { return this.inactiveJobs; } }
        [JsonProperty] private readonly List<JobInfo> inactiveJobs = new();

        [JsonIgnore] public IEnumerable<StatusEffectInfo> StatusEffects { get { return this.statusEffects; } }
        [JsonProperty] private readonly List<StatusEffectInfo> statusEffects = new();

        [JsonIgnore] public int ItemsCount { get { return this.items.Count; } }
        [JsonIgnore] public IEnumerable<ItemInfo> Items { get { return this.items; } }
        [JsonProperty] private readonly List<ItemInfo> items = new();
        [JsonIgnore] public int EquipmentsCount { get { return this.equipments.Count; } }
        [JsonIgnore] public IEnumerable<EquipmentInfo> Equipments { get { return this.equipments; } }
        [JsonProperty] private readonly List<EquipmentInfo> equipments = new();

        [JsonIgnore] public Combat.CharacterCombatAI CombatAI { get { return this.combatAI; } }
        [JsonProperty] private readonly Combat.CharacterCombatAI combatAI = new();
        [JsonIgnore] public List<EntityLoot> ItemsLoot { get { return this.itemsLoot; } }
        [JsonProperty] private readonly List<EntityLoot> itemsLoot = new();
        [JsonProperty] private readonly List<EntityLoot> equipmentsLoot = new();
        [JsonIgnore] public int CurrencyLoot { get { return this.currencyLoot; } }
        [JsonProperty] private readonly int currencyLoot = 1;

        #region Stats Properties
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
            //Console.Log($"Health Before: {this.Health + value} Health After: {this.Health}");
            return value;
        }

        public int DirectDamage(int damage)
        {
            int value = damage > 0 ? damage : 1;
            this.health -= value;
            //Console.Log($"Health Before: {this.Health + value} Health After: {this.Health}");
            return value;
        }

        public int CriticalDamage(int damage)
        {
            int value = damage > 0 ? damage * this.CurrentJob.CritMultiplier : 1;
            this.health -= value;
            //Console.Log($"Health Before: {this.Health + value} Health After: {this.Health}");
            return value;
        }

        public int PsychicDamage(int damage)
        {
            int value = (damage - this.Focus) > 0 ? (damage - this.Focus) : 1;
            this.health -= value;
            //Console.Log($"Health Before: {this.Health + value} Health After: {this.Health}");
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
            //Console.Log($"Health Before: {this.Health - value} Health After: {this.Health}");
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
            //Console.Log($"Stamina Before: {this.Stamina - value} Stamina After: {this.Stamina}");
            return value;
        }

        public int Exert(int exertion)
        {
            int value = exertion > 0 ? exertion : 1;
            this.stamina -= value;
            //Console.Log($"Stamina Before: {this.Stamina + value} Stamina After: {this.Stamina}");
            return value;
        }

        public int ResetHealth()
        {
            int healthBefore = this.Health;
            this.health = this.MaxHealth;
            //Console.Log($"Health Before: {healthBefore} Health After: {this.Health}");
            return this.Health - healthBefore;
        }

        public int ResetStamina()
        {
            int staminaBefore = this.Stamina;
            this.stamina = this.MaxStamina;
            //Console.Log($"Stamina Before: {staminaBefore} Stamina After: {this.Stamina}");
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
            this.constitution += value;
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
            this.will += value;
            return value;
        }
        #endregion

        #region Generic Public Utilities
        public CharacterStats Setup()
        {
            foreach (EquipmentType equipmentType in Enum.GetValues(typeof(EquipmentType)))
                if (IsEquipped(equipmentType))
                    EquipModifierUpdate(GameInfo.Instance.GetEquipment(GetEquipped(equipmentType)));

            foreach (StatusEffectInfo info in this.statusEffects)
            {
                ApplyStatusEffectModifierUpdate(info.Get());
            }

            return this;
        }

        public void SetName(string newName)
        {
            this.name = newName;
        }

        public string GetStatsPropertyText(CharacterStatsProperty property)
        {
            return $"{GetStatsProperty(property)}{Strings.CharacterStats.GetStatsPropertySymbol(property)}";
        }

        public JobInfo.LevelUpInfo GrantExperience(int experience)
        {
            return this.CurrentJob.GrantExperience(this, experience);
        }
        #endregion

        #region Items
        public ItemInfo AcquireItem(string id)
        {
            ItemInfo info = AddItem(id);

            if (info is null)
            {
                Terminal.IO.LogError($"[CharacterStats:AddEffect] Tried to add an item to {this.name}'s itemTemplates bag but the item id given was invalid");
                return null;
            }

            info.Increment();
            return info;
        }

        public ItemInfo RemoveItem(ItemInfo info)
        {
            if (info is null)
            {
                Terminal.IO.LogError($"[CharacterStats:AddEffect] Tried to remove an item from {this.name}'s item bag but the item info given was null");
                return null;
            }

            this.items.Remove(info);
            return info;
        }

        private ItemInfo AddItem(string id)
        {
            if (id == string.Empty)
                return null;

            foreach (ItemInfo info in this.items)
            {
                if (info.ID == id)
                    return info;
            }

            ItemInfo newInfo = new(id);
            this.items.Add(newInfo);
            return newInfo;
        }
        #endregion

        #region Equipments
        public void Equip(EquipmentInfo info)
        {
            Equipment equipment = info.Get();

            if (equipment is null)
            {
                Terminal.IO.LogWarning($"[CharacterStats:Equip] Tried to equip to {this.name} but the equipment given was null");
                return;
            }

            EquipModifierUpdate(equipment);
            info.Decrement(this);
            EquipByType(equipment.Type, info.ID);
        }

        public void Unequip(EquipmentType equipmentType)
        {
            string id = GetEquipped(equipmentType);
            UnequipModifierUpdate(GameInfo.Instance.GetEquipment(id));
            EquipByType(equipmentType, null);
            AcquireEquipment(id);
        }

        public bool IsEquipped(EquipmentType equipmentType)
        {
            return GetEquipped(equipmentType) != null && GetEquipped(equipmentType) != string.Empty;
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

        public EquipmentInfo AcquireEquipment(string id)
        {
            EquipmentInfo info = AddEquipment(id);
            
            if (info is null)
            {
                Terminal.IO.LogError($"[CharacterStats:AddEffect] Tried to add an equipment to {this.name}'s equipments bag but the equipment id given was invalid");
                return null;
            }

            info.Increment();
            return info;
        }

        public EquipmentInfo RemoveEquipment(EquipmentInfo info)
        {
            if (info is null)
            {
                Terminal.IO.LogError($"[CharacterStats:AddEffect] Tried to remove an equipment from {this.name}'s equipments bag but the equipment info given was null");
                return null;
            }

            UnequipModifierUpdate(info.Get());
            this.equipments.Remove(info);
            return info;
        }

        private EquipmentInfo AddEquipment(string id)
        {
            if (id == string.Empty)
                return null;

            foreach (EquipmentInfo info in this.equipments)
            {
                if (info.ID == id)
                    return info;
            }

            EquipmentInfo newInfo = new(id);
            this.equipments.Add(newInfo);
            return newInfo;
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
                default: Terminal.IO.LogError($"[CharacterStats:SetEquipmentByType] Tried to equip an equipment that didn't have a type to {this.name}"); return;
            }
        }

        private void EquipModifierUpdate(Equipment equipment)
        {
            if (equipment == null)
                return;

            this.speedModifier.Equip(equipment);
            this.attackModifier.Equip(equipment);
            this.defenseModifier.Equip(equipment);
            this.focusModifier.Equip(equipment);
            this.luckModifier.Equip(equipment);
            this.maxHealthModifier.Equip(equipment);
            this.maxStaminaModifier.Equip(equipment);

            //Terminal.IO.Log($"{this.speedModifier.Total} {this.attackModifier.Total} {this.defenseModifier.Total} {this.focusModifier.Total} {this.luckModifier.Total} {this.maxHealthModifier.Total} {this.maxStaminaModifier.Total}");
        }

        private void UnequipModifierUpdate(Equipment equipment)
        {
            if (equipment == null)
                return;

            this.speedModifier.Unequip(equipment);
            this.attackModifier.Unequip(equipment);
            this.defenseModifier.Unequip(equipment);
            this.focusModifier.Unequip(equipment);
            this.luckModifier.Unequip(equipment);
            this.maxHealthModifier.Unequip(equipment);
            this.maxStaminaModifier.Unequip(equipment);

            //Terminal.IO.Log($"{this.speedModifier.Total} {this.attackModifier.Total} {this.defenseModifier.Total} {this.focusModifier.Total} {this.luckModifier.Total} {this.maxHealthModifier.Total} {this.maxStaminaModifier.Total}");
        }
        #endregion

        #region Status Effects
        public void ApplyStatusEffect(string id)
        {
            StatusEffectInfo info = AddStatusEffect(id);

            if (info is null)
            {
                Terminal.IO.LogError($"[CharacterStats:ApplyStatusEffect] Tried to apply a status effect to {this.name} but the status effect id given was invalid");
                return;
            }

            ApplyStatusEffectModifierUpdate(info.Get());
        }

        public void RemoveStatusEffect(StatusEffectInfo info)
        {
            if (info is null)
            {
                Terminal.IO.LogError($"[CharacterStats:RemoveStatusEffect] Tried to remove a status effect from {this.name} but the status effect info given was null");
                return;
            }

            RemoveStatusEffectModifierUpdate(info.Get());
            this.statusEffects.Remove(info);
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
            this.maxHealthModifier.ApplyStatusEffect(statusEffect);
            this.maxStaminaModifier.ApplyStatusEffect(statusEffect);

            //Terminal.IO.Log($"{this.speedModifier.Total} {this.attackModifier.Total} {this.defenseModifier.Total} {this.focusModifier.Total} {this.luckModifier.Total} {this.maxHealthModifier.Total} {this.maxStaminaModifier.Total}");
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
            this.maxHealthModifier.RemoveStatusEffect(statusEffect);
            this.maxStaminaModifier.RemoveStatusEffect(statusEffect);

            //Terminal.IO.Log($"{this.speedModifier.Total} {this.attackModifier.Total} {this.defenseModifier.Total} {this.focusModifier.Total} {this.luckModifier.Total} {this.maxHealthModifier.Total} {this.maxStaminaModifier.Total}");
        }

        private StatusEffectInfo AddStatusEffect(string id)
        {
            if (id == string.Empty)
                return null;

            StatusEffectInfo newInfo = new(id);
            this.statusEffects.Add(newInfo);
            return newInfo;
        }
        #endregion
    }
}
