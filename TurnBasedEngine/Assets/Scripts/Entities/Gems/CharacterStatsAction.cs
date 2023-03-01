using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using BF2D.Enums;

namespace BF2D.Game.Actions
{
    /// <summary>
    /// Gem
    /// </summary>
    [Serializable]
    public class CharacterStatsAction : Entity, IUtilityEntity
    {
        public class Info
        {
            public string message = string.Empty;
            public bool targetWasKilled = false;
            public bool targetWasRevived = false;
            public CharacterStats target = null;
            public bool failed = false;

            public string GetMessage()
            {
                if (this.failed)
                    return $"But {this.target.Name} was not affected. [P:0.2]";
                return message;
            }
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
        [JsonIgnore] public int SuccessRate { get { return this.successRate; } }
        [JsonProperty] private readonly int successRate = 100;
        [JsonIgnore] public CombatAlignment Alignment { get { return this.alignment; } }
        [JsonProperty] public CombatAlignment alignment = CombatAlignment.Neutral;

        public string GetAnimationKey()
        {
            if (this.criticalDamage is not null || this.psychicDamage is not null || this.directDamage is not null || this.damage is not null)
                return Strings.Animation.Damaged;

            return Strings.Animation.Flashing;
        }

        public Info Run(CharacterStats source, CharacterStats target)
        {
            Info info = new()
            {
                target = target
            };

            int randomValue = UnityEngine.Random.Range(0, 100);
            if (randomValue > this.successRate)
            {
                info.failed = true;
                return info;
            }

            bool targetDeadPrevious = target.Dead;

            if (this.resetHealth)
            {
                info.message += $"{target.Name}'s {BF2D.Game.Strings.CharacterStats.Health} went up to full. [P:0.2]";
                target.ResetHealth();
            }
            if (this.resetStamina)
            {
                info.message += $"{target.Name}'s {BF2D.Game.Strings.CharacterStats.Stamina} went up to full. [P:0.2]";
                target.ResetStamina();
            }

            if (this.damage is not null)
            {
                if (UnityEngine.Random.Range(0, 100) < source.CurrentJob.CritChance && !target.CritImmune)
                    info.message += $"{BF2D.Game.Strings.CharacterStats.CriticalDamage}.[P:0.2] {target.Name} took {RunCharacterStatsActionProperty(this.damage, source, target.CriticalDamage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}. [P:0.2]";
                else
                    info.message += $"{target.Name} took {RunCharacterStatsActionProperty(this.damage, source, target.Damage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}. [P:0.2]";
            }
            if (this.directDamage is not null)
            {
                if (UnityEngine.Random.Range(0, 100) < source.CurrentJob.CritChance && !target.CritImmune)
                    info.message += $"{BF2D.Game.Strings.CharacterStats.CriticalDamage}.[P:0.2] {target.Name} took {RunCharacterStatsActionProperty(this.directDamage, source, target.CriticalDamage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}. [P:0.2]";
                else
                    info.message += $"{target.Name} took {RunCharacterStatsActionProperty(this.directDamage, source, target.DirectDamage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}. [P:0.2]";
            }
            if (this.criticalDamage is not null)
                info.message += $"{BF2D.Game.Strings.CharacterStats.CriticalDamage}.[P:0.2] {target.Name} took {RunCharacterStatsActionProperty(this.criticalDamage, source, target.CriticalDamage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}. [P:0.2]";
            if (this.psychicDamage is not null)
                info.message += $"{target.Name} took {RunCharacterStatsActionProperty(this.psychicDamage, source, target.PsychicDamage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}. [P:0.2]";
            if (this.heal is not null)
                info.message += $"{target.Name} gained {RunCharacterStatsActionProperty(this.heal, source, target.Heal)} {BF2D.Game.Strings.CharacterStats.Health.ToLower()}. [P:0.2]";
            if (this.recover is not null)
                info.message += $"{target.Name} recovered {RunCharacterStatsActionProperty(this.recover, source, target.Recover)} {BF2D.Game.Strings.CharacterStats.Stamina.ToLower()}. [P:0.2]";
            if (this.exert is not null)
                info.message += $"{target.Name} exerted {RunCharacterStatsActionProperty(this.exert, source, target.Exert)} {BF2D.Game.Strings.CharacterStats.Stamina.ToLower()}. [P:0.2]";
            if (this.statusEffect is not null)
            {
                StatusEffect statusEffect = GameInfo.Instance.GetStatusEffect(this.statusEffect);
                if (statusEffect is null)
                {
                    Terminal.IO.LogError($"[CharacterStatsAction:Run] A status effect with id '{this.statusEffect}' does not exist");
                }
                else
                {
                    info.message += source == target ?
                        $"{source.Name} {statusEffect.Description} themself with {statusEffect.Name}. [P:0.2]" :
                        $"{source.Name} {statusEffect.Description} {target.Name} with {statusEffect.Name}. [P:0.2]";
                    target.ApplyStatusEffect(this.statusEffect);
                }
            }

            info.targetWasKilled = !targetDeadPrevious && target.Dead;
            info.targetWasRevived = targetDeadPrevious && !target.Dead;

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