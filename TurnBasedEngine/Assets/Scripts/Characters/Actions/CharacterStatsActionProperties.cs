using UnityEngine;
using System;
using System.Collections.Generic;
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

        public List<string> Run(CharacterStats source, CharacterStats target)
        {
            List<string> message = new();
            if (this.resetHealth)
            {
                message.Add($"{target.Name}'s {BF2D.Game.Strings.CharacterStats.Health} goes up to full");
                target.ResetHealth();
            }
            if (this.resetStamina)
            {
                message.Add($"{target.Name}'s {BF2D.Game.Strings.CharacterStats.Stamina} goes up to full");
                target.ResetStamina();
            }

            if (this.damage is not null)
                message.Add($"{target.Name} takes {RunCharacterStatsActionProperty(this.damage, source, target.Damage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}");
            if (this.criticalDamage is not null)
                message.Add($"{BF2D.Game.Strings.CharacterStats.CriticalDamage}. {target.Name} takes {RunCharacterStatsActionProperty(this.criticalDamage, source, target.CriticalDamage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}");
            if (this.psychicDamage is not null)
                message.Add($"{target.Name} takes {RunCharacterStatsActionProperty(this.psychicDamage, source, target.PsychicDamage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}.");
            if (this.heal is not null)
                message.Add($"{target.Name} gains {RunCharacterStatsActionProperty(this.heal, source, target.Heal)} {BF2D.Game.Strings.CharacterStats.Health.ToLower()}");
            if (this.recover is not null)
                message.Add($"{target.Name} recovers {RunCharacterStatsActionProperty(this.recover, source, target.Recover)} {BF2D.Game.Strings.CharacterStats.Stamina.ToLower()}");
            if (this.Exert is not null)
                message.Add($"{target.Name} exerts {RunCharacterStatsActionProperty(this.exert, source, target.Exert)} {BF2D.Game.Strings.CharacterStats.Stamina.ToLower()}");

            return message;
        }

        private int RunCharacterStatsActionProperty(CharacterStatsActionProperty statsActionProperty, CharacterStats source, Action<int> targetAction)
        {
            int value = statsActionProperty.Calculate(source);

            targetAction(value);

            return value;
        }
    }
}
