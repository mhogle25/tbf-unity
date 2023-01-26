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
        [JsonProperty] private readonly CharacterStatsActionProperty damage = null;
        [JsonIgnore] public CharacterStatsActionProperty DirectDamage { get { return this.directDamage; } }
        [JsonProperty] private readonly CharacterStatsActionProperty directDamage = null;
        [JsonIgnore] public CharacterStatsActionProperty CriticalDamage { get { return this.criticalDamage; } }
        [JsonProperty] private readonly CharacterStatsActionProperty criticalDamage = null;
        [JsonIgnore] public CharacterStatsActionProperty PsychicDamage { get { return this.psychicDamage; } }
        [JsonProperty] private readonly CharacterStatsActionProperty psychicDamage = null;
        [JsonIgnore] public CharacterStatsActionProperty Heal { get { return this.heal; } }
        [JsonProperty] private readonly CharacterStatsActionProperty heal = null;
        [JsonIgnore] public CharacterStatsActionProperty Recover { get { return this.recover; } }
        [JsonProperty] private readonly CharacterStatsActionProperty recover = null;
        [JsonIgnore] public CharacterStatsActionProperty Exert { get { return this.exert; } }
        [JsonProperty] private readonly CharacterStatsActionProperty exert = null;
        [JsonIgnore] public bool ResetHealth { get { return this.resetHealth; } }
        [JsonProperty] private readonly bool resetHealth = false;
        [JsonIgnore] public bool ResetStamina { get { return this.resetStamina; } }
        [JsonProperty] private readonly bool resetStamina = false;
        [JsonIgnore] public StatusEffect StatusEffect { get { return this.statusEffect; } }
        [JsonProperty] private readonly StatusEffect statusEffect = null;

        public string MessageRun(CharacterStats source, CharacterStats target)
        {
            string message = string.Empty;
            foreach (string s in DialogRun(source, target))
            {
                message += $"{s}\n";
            };
            return message;
        }
         
        public List<string> DialogRun(CharacterStats source, CharacterStats target)
        {
            List<string> message = new();
            if (this.resetHealth)
            {
                message.Add($"{target.Name}'s {BF2D.Game.Strings.CharacterStats.Health} went up to full.");
                target.ResetHealth();
            }
            if (this.resetStamina)
            {
                message.Add($"{target.Name}'s {BF2D.Game.Strings.CharacterStats.Stamina} went up to full.");
                target.ResetStamina();
            }

            if (this.damage is not null)
                message.Add($"{target.Name} took {RunCharacterStatsActionProperty(this.damage, source, target.Damage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}.");
            if (this.directDamage is not null)
                message.Add($"{target.Name} took {RunCharacterStatsActionProperty(this.directDamage, source, target.DirectDamage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}.");
            if (this.criticalDamage is not null)
                message.Add($"{BF2D.Game.Strings.CharacterStats.CriticalDamage}.[P:0.2] {target.Name} took {RunCharacterStatsActionProperty(this.criticalDamage, source, target.CriticalDamage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}.");
            if (this.psychicDamage is not null)
                message.Add($"{target.Name} took {RunCharacterStatsActionProperty(this.psychicDamage, source, target.PsychicDamage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}.");
            if (this.heal is not null)
                message.Add($"{target.Name} gained {RunCharacterStatsActionProperty(this.heal, source, target.Heal)} {BF2D.Game.Strings.CharacterStats.Health.ToLower()}.");
            if (this.recover is not null)
                message.Add($"{target.Name} recovered {RunCharacterStatsActionProperty(this.recover, source, target.Recover)} {BF2D.Game.Strings.CharacterStats.Stamina.ToLower()}.");
            if (this.exert is not null)
                message.Add($"{target.Name} exerted {RunCharacterStatsActionProperty(this.exert, source, target.Exert)} {BF2D.Game.Strings.CharacterStats.Stamina.ToLower()}.");

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
