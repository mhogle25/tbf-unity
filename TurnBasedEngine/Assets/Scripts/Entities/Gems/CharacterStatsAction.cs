using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class CharacterStatsAction : Entity
    {
        public class Info
        {
            public string message = string.Empty;
            public bool targetWasKilled = false;
            public bool targetWasRevived = false;
            public CharacterStats target = null;
        }

        private delegate int CalculatedAction(int value);

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
        [JsonIgnore] public string StatusEffect { get { return this.statusEffect; } }
        [JsonProperty] private readonly string statusEffect = null;

        public string GetAnimationKey()
        {
            if (this.criticalDamage is not null || this.psychicDamage is not null || this.directDamage is not null || this.damage is not null)
                return Strings.Animation.Damaged;

            return Strings.Animation.Flashing;
        }
        
        public Info Run(CharacterStats source, CharacterStats target)
        {
            Info info = new();
            bool targetDeadPrevious = target.Dead;

            if (this.resetHealth)
            {
                info.message += $"{target.Name}'s {BF2D.Game.Strings.CharacterStats.Health} went up to full. [P:0.1]";
                target.ResetHealth();
            }
            if (this.resetStamina)
            {
                info.message += $"{target.Name}'s {BF2D.Game.Strings.CharacterStats.Stamina} went up to full. [P:0.1]";
                target.ResetStamina();
            }

            if (this.damage is not null)
                info.message += $"{target.Name} took {RunCharacterStatsActionProperty(this.damage, source, target.Damage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}. [P:0.1]";
            if (this.directDamage is not null)
                info.message += $"{target.Name} took {RunCharacterStatsActionProperty(this.directDamage, source, target.DirectDamage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}. [P:0.1]";
            if (this.criticalDamage is not null)
                info.message += $"{BF2D.Game.Strings.CharacterStats.CriticalDamage}.[P:0.2] {target.Name} took {RunCharacterStatsActionProperty(this.criticalDamage, source, target.CriticalDamage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}. [P:0.1]";
            if (this.psychicDamage is not null)
                info.message += $"{target.Name} took {RunCharacterStatsActionProperty(this.psychicDamage, source, target.PsychicDamage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}. [P:0.1]";
            if (this.heal is not null)
                info.message += $"{target.Name} gained {RunCharacterStatsActionProperty(this.heal, source, target.Heal)} {BF2D.Game.Strings.CharacterStats.Health.ToLower()}. [P:0.1]";
            if (this.recover is not null)
                info.message += $"{target.Name} recovered {RunCharacterStatsActionProperty(this.recover, source, target.Recover)} {BF2D.Game.Strings.CharacterStats.Stamina.ToLower()}. [P:0.1]";
            if (this.exert is not null)
                info.message += $"{target.Name} exerted {RunCharacterStatsActionProperty(this.exert, source, target.Exert)} {BF2D.Game.Strings.CharacterStats.Stamina.ToLower()}. [P:0.1]";
            if (this.statusEffect is not null)
            {
                StatusEffect statusEffect = GameInfo.Instance.GetStatusEffect(this.statusEffect);
                if (statusEffect is null)
                    return info;
                info.message += source == target ? 
                    $"{source.Name} {statusEffect.Description} themself with {statusEffect.Name}. [P:0.1]" : 
                    $"{source.Name} {statusEffect.Description} {target.Name} with {statusEffect.Name}. [P:0.1]";
                target.ApplyStatusEffect(this.statusEffect);
            }

            info.targetWasKilled = !targetDeadPrevious && target.Dead;
            info.targetWasRevived = targetDeadPrevious && !target.Dead;
            info.target = target;

            return info;
        }

        private int RunCharacterStatsActionProperty(CharacterStatsActionProperty statsActionProperty, CharacterStats source, CalculatedAction targetAction)
        {
            int value = statsActionProperty.Calculate(source);

            int result = targetAction(value);

            return result;
        }
    }
}