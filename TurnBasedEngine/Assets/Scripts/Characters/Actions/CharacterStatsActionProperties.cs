using UnityEngine;
using System;
using Newtonsoft.Json;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class CharacterStatsActionProperties
    {
        [JsonIgnore] public CharacterStatsActionProperty Damage { get { return this.damage; } }
        [JsonProperty] private CharacterStatsActionProperty damage = null;
        [JsonIgnore] public CharacterStatsActionProperty CriticalDamage { get { return this.criticalDamage; } }
        [JsonProperty] private CharacterStatsActionProperty criticalDamage = null;
        [JsonIgnore] public CharacterStatsActionProperty PsychicDamage { get { return this.psychicDamage; } }
        [JsonProperty] private CharacterStatsActionProperty psychicDamage = null;
        [JsonIgnore] public CharacterStatsActionProperty Heal { get { return this.heal; } }
        [JsonProperty] private CharacterStatsActionProperty heal = null;
        [JsonIgnore] public CharacterStatsActionProperty Recover { get { return this.recover; } }
        [JsonProperty] private CharacterStatsActionProperty recover = null;
        [JsonIgnore] public CharacterStatsActionProperty Exert { get { return this.exert; } }
        [JsonProperty] private CharacterStatsActionProperty exert = null;
        [JsonIgnore] public bool ResetHealth { get { return this.resetHealth; } }
        [JsonProperty] private bool resetHealth = false;
        [JsonIgnore] public bool ResetStamina { get { return this.resetStamina; } }
        [JsonProperty] private bool resetStamina = false;
        [JsonIgnore] public string StatusEffect { get { return this.statusEffect; } }
        [JsonProperty] private string statusEffect = null;

        public void Run(CharacterStats character)
        {
            if (this.resetHealth)
                character.ResetHealth();
            if (this.resetStamina)
                character.ResetStamina();

            RunCharacterStatsActionProperty(character, this.damage, character.Damage);
            RunCharacterStatsActionProperty(character, this.criticalDamage, character.CriticalDamage);
            RunCharacterStatsActionProperty(character, this.psychicDamage, character.PsychicDamage);
            RunCharacterStatsActionProperty(character, this.heal, character.Heal);
            RunCharacterStatsActionProperty(character, this.recover, character.Recover);
            RunCharacterStatsActionProperty(character, this.exert, character.Exert);
        }

        private void RunCharacterStatsActionProperty(CharacterStats character, CharacterStatsActionProperty statsActionProperty, Action<int> action)
        {
            if (statsActionProperty is null)
                return;

            action(statsActionProperty.Calculate(character));
        }
    }
}
