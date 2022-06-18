using UnityEngine;
using System;
using Newtonsoft.Json;

namespace BF2D.Actions
{
    [Serializable]
    public class CharacterStatsActionProperties
    {
        [JsonIgnore] public int Damage { get { return this.damage; } }
        [JsonProperty] private int damage = 0;
        [JsonIgnore] public int CriticalDamage { get { return this.criticalDamage; } }
        [JsonProperty] private int criticalDamage = 0;
        [JsonIgnore] public int PsychicDamage { get { return this.psychicDamage; } }
        [JsonProperty] private int psychicDamage = 0;
        [JsonIgnore] public int Heal { get { return this.heal; } }
        [JsonProperty] private int heal = 0;
        [JsonIgnore] public int Recover { get { return this.recover; } }
        [JsonProperty] private int recover = 0;
        [JsonIgnore] public int Exert { get { return this.exert; } }
        [JsonProperty] private int exert = 0;
        [JsonIgnore] public bool ResetHealth { get { return this.resetHealth; } }
        [JsonProperty] private bool resetHealth = false;
        [JsonIgnore] public bool ResetStamina { get { return this.resetStamina; } }
        [JsonProperty] private bool resetStamina = false;
        [JsonIgnore] public string StatusEffect { get { return this.statusEffect; } }
        [JsonProperty] private string statusEffect = null;
    }

}
