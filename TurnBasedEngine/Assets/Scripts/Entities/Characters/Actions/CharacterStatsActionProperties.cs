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
         
        public string Run(CharacterStats source, CharacterStats target)
        {
            string actionMessage = string.Empty;

            if (this.resetHealth)
            {
                actionMessage += $"{target.Name}'s {BF2D.Game.Strings.CharacterStats.Health} went up to full. ";
                target.ResetHealth();
            }
            if (this.resetStamina)
            {
                actionMessage += $"{target.Name}'s {BF2D.Game.Strings.CharacterStats.Stamina} went up to full. ";
                target.ResetStamina();
            }

            if (this.damage is not null)
                actionMessage += $"{target.Name} took {RunCharacterStatsActionProperty(this.damage, source, target.Damage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}. ";
            if (this.directDamage is not null)
                actionMessage += $"{target.Name} took {RunCharacterStatsActionProperty(this.directDamage, source, target.DirectDamage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}. ";
            if (this.criticalDamage is not null)
                actionMessage += $"{BF2D.Game.Strings.CharacterStats.CriticalDamage}.[P:0.2] {target.Name} took {RunCharacterStatsActionProperty(this.criticalDamage, source, target.CriticalDamage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}. ";
            if (this.psychicDamage is not null)
                actionMessage += $"{target.Name} took {RunCharacterStatsActionProperty(this.psychicDamage, source, target.PsychicDamage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}. ";
            if (this.heal is not null)
                actionMessage += $"{target.Name} gained {RunCharacterStatsActionProperty(this.heal, source, target.Heal)} {BF2D.Game.Strings.CharacterStats.Health.ToLower()}. ";
            if (this.recover is not null)
                actionMessage += $"{target.Name} recovered {RunCharacterStatsActionProperty(this.recover, source, target.Recover)} {BF2D.Game.Strings.CharacterStats.Stamina.ToLower()}. ";
            if (this.exert is not null)
                actionMessage += $"{target.Name} exerted {RunCharacterStatsActionProperty(this.exert, source, target.Exert)} {BF2D.Game.Strings.CharacterStats.Stamina.ToLower()}. ";
            if (this.statusEffect is not null)
            {
                actionMessage += source == target ? $"{source.Name} {this.statusEffect.Description} themself with {this.statusEffect.Name}. " : $"{source.Name} {this.statusEffect.Description} {target.Name} with {this.statusEffect.Name}. ";
                target.ApplyStatusEffect(this.statusEffect);
            }

            return actionMessage;
        }

        private int RunCharacterStatsActionProperty(CharacterStatsActionProperty statsActionProperty, CharacterStats source, Action<int> targetAction)
        {
            int value = statsActionProperty.Calculate(source);

            targetAction(value);

            return value;
        }

        public string GetAnimationKey()
        {
            if (this.criticalDamage is not null || this.psychicDamage is not null || this.directDamage is not null || this.damage is not null)
                return Strings.Animation.Damaged;

            return Strings.Animation.Flashing;
        }
    }
}
