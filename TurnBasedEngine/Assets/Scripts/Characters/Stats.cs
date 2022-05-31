using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace BF2D.Game
{
    [Serializable]
    public abstract class Stats
    {
        [JsonIgnore] public string Name { get { return this.name; } set { this.name = value; } }
        [JsonProperty] private string name = string.Empty;
        [JsonIgnore] public string Description { get { return this.description; } set { this.description = value; } }
        [JsonProperty] private string description = string.Empty;
        [JsonIgnore] public int Health { get { return this.health; } }
        [JsonProperty] private int health = 0;
        [JsonIgnore] public int MaxHealth { get { return this.maxHealth; } }
        [JsonProperty] private int maxHealth = 0;
        [JsonIgnore] public int Stamina { get { return this.stamina; } }
        [JsonProperty] private int stamina = 0;
        [JsonIgnore] public int MaxStamina { get { return this.maxStamina; } }
        [JsonProperty] private int maxStamina = 0;
        [JsonIgnore] public int Speed { get { return this.speed + this.swiftness; } }
        [JsonProperty] private int speed = 0;
        [JsonIgnore] public int Swiftness { get { return this.swiftness; } }
        [JsonProperty] private int swiftness = 0;
        [JsonIgnore] public int Attack { get { return this.attack + this.strength; } }
        [JsonProperty] private int attack = 0;
        [JsonIgnore] public int Strength { get { return this.strength; } }
        [JsonProperty] private int strength = 0;
        [JsonIgnore] public int Defense { get { return this.defense + this.toughness; } }
        [JsonProperty] private int defense = 0;
        [JsonIgnore] public int Toughness { get { return this.toughness; } }
        [JsonProperty] private int toughness = 0;
        [JsonIgnore] public int Focus { get { return this.focus + this.will; } }
        [JsonProperty] private int focus = 0;
        [JsonIgnore] public int Will { get { return this.will; } }
        [JsonProperty] private int will = 0;
        [JsonIgnore] public int Luck { get { return this.luck; } }
        [JsonProperty] private int luck = 0;
        [JsonIgnore] public List<Item> Inventory { get { return this.inventory; } }
        [JsonProperty] private List<Item> inventory = new List<Item>();

        public void Damage(int damage)
        {
            this.health -= (damage - this.Defense) > 0 ? (damage - this.Defense) : 1;
        }

        public void CriticalDamage(int damage)
        {
            this.health -= damage > 0 ? damage : 1;
        }

        public void PsychicDamage(int damage)
        {
            this.health -= (damage - this.Focus) > 0 ? (damage - this.Focus) : 1;
        }

        public void Heal(int healing)
        {
            this.health += healing > 0 ? healing : 1;
        }

        public void Recover(int recovery)
        {
            this.stamina += recovery > 0 ? recovery : 1;
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

        public void ResetStats()
        {
            ResetHealth();
            ResetStamina();
        }
    }
}
