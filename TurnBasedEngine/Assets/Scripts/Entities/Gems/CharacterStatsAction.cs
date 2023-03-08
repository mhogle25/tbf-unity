using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.TextCore.Text;

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

        [JsonIgnore] public NumericProperty Damage { get { return this.damage; } }
        [JsonProperty] private readonly NumericProperty damage = null;
        [JsonIgnore] public NumericProperty DirectDamage { get { return this.directDamage; } }
        [JsonProperty] private readonly NumericProperty directDamage = null;
        [JsonIgnore] public NumericProperty CriticalDamage { get { return this.criticalDamage; } }
        [JsonProperty] private readonly NumericProperty criticalDamage = null;
        [JsonIgnore] public NumericProperty PsychicDamage { get { return this.psychicDamage; } }
        [JsonProperty] private readonly NumericProperty psychicDamage = null;
        [JsonIgnore] public NumericProperty Heal { get { return this.heal; } }
        [JsonProperty] private readonly NumericProperty heal = null;
        [JsonIgnore] public NumericProperty Recover { get { return this.recover; } }
        [JsonProperty] private readonly NumericProperty recover = null;
        [JsonIgnore] public NumericProperty Exert { get { return this.exert; } }
        [JsonProperty] private readonly NumericProperty exert = null;
        [JsonIgnore] public bool ResetHealth { get { return this.resetHealth; } }
        [JsonProperty] private readonly bool resetHealth = false;
        [JsonIgnore] public bool ResetStamina { get { return this.resetStamina; } }
        [JsonProperty] private readonly bool resetStamina = false;
        [JsonIgnore] public StatusEffectProperty StatusEffect { get { return this.statusEffect; } }
        [JsonProperty] private readonly StatusEffectProperty statusEffect = null;
        [JsonIgnore] public NumericProperty ConstitutionUp { get { return this.constitutionUp; } }
        [JsonProperty] private readonly NumericProperty constitutionUp = null;
        [JsonIgnore] public NumericProperty EnduranceUp { get { return this.enduranceUp; } }
        [JsonProperty] private readonly NumericProperty enduranceUp = null;
        [JsonIgnore] public NumericProperty SwiftnessUp { get { return this.swiftnessUp; } }
        [JsonProperty] private readonly NumericProperty swiftnessUp = null;
        [JsonIgnore] public NumericProperty StrengthUp { get { return this.strengthUp; } }
        [JsonProperty] private readonly NumericProperty strengthUp = null;
        [JsonIgnore] public NumericProperty ToughnessUp { get { return this.toughnessUp; } }
        [JsonProperty] private readonly NumericProperty toughnessUp = null;
        [JsonIgnore] public NumericProperty WillUp { get { return this.willUp; } }
        [JsonProperty] private readonly NumericProperty willUp = null;
        [JsonIgnore] public NumericProperty FortuneUp { get { return this.fortuneUp; } }
        [JsonProperty] private readonly NumericProperty fortuneUp = null;

        [JsonIgnore] public int SuccessRate { get { return this.successRate; } }
        [JsonProperty] private readonly int successRate = 100;
        [JsonIgnore] public Enums.CombatAlignment Alignment { get { return this.alignment; } }
        [JsonProperty] private Enums.CombatAlignment alignment = Enums.CombatAlignment.Neutral;

        #region Public Methods
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

            if (!Utilities.Probability.Roll(source, this.successRate))
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
                if (Utilities.Probability.Roll(source, source.CurrentJob.CritChance) && !target.CritImmune)
                    info.message += $"{BF2D.Game.Strings.CharacterStats.CriticalDamage}.[P:0.2] {target.Name} took {RunNumericProperty(this.damage, source, target.CriticalDamage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}. [P:0.2]";
                else
                    info.message += $"{target.Name} took {RunNumericProperty(this.damage, source, target.Damage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}. [P:0.2]";
            }
            if (this.directDamage is not null)
            {
                if (Utilities.Probability.Roll(source, source.CurrentJob.CritChance) && !target.CritImmune)
                    info.message += $"{BF2D.Game.Strings.CharacterStats.CriticalDamage}.[P:0.2] {target.Name} took {RunNumericProperty(this.directDamage, source, target.CriticalDamage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}. [P:0.2]";
                else
                    info.message += $"{target.Name} took {RunNumericProperty(this.directDamage, source, target.DirectDamage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}. [P:0.2]";
            }
            if (this.criticalDamage is not null)
                info.message += $"{BF2D.Game.Strings.CharacterStats.CriticalDamage}.[P:0.2] {target.Name} took {RunNumericProperty(this.criticalDamage, source, target.CriticalDamage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}. [P:0.2]";
            if (this.psychicDamage is not null)
                info.message += $"{target.Name} took {RunNumericProperty(this.psychicDamage, source, target.PsychicDamage)} {BF2D.Game.Strings.CharacterStats.Damage.ToLower()}. [P:0.2]";
            if (this.heal is not null)
                info.message += $"{target.Name} gained {RunNumericProperty(this.heal, source, target.Heal)} {BF2D.Game.Strings.CharacterStats.Health.ToLower()}. [P:0.2]";
            if (this.recover is not null)
                info.message += $"{target.Name} recovered {RunNumericProperty(this.recover, source, target.Recover)} {BF2D.Game.Strings.CharacterStats.Stamina.ToLower()}. [P:0.2]";
            if (this.exert is not null)
                info.message += $"{target.Name} exerted {RunNumericProperty(this.exert, source, target.Exert)} {BF2D.Game.Strings.CharacterStats.Stamina.ToLower()}. [P:0.2]";
            if (this.statusEffect is not null)
            {
                StatusEffect statusEffect = this.statusEffect.Get();
                if (statusEffect is null)
                {
                    Terminal.IO.LogError($"[CharacterStatsAction:Run] A status effect with id '{this.statusEffect}' does not exist");
                }
                else
                {
                    if (Utilities.Probability.Roll(source, this.statusEffect.successRate))
                    {
                        if (target.ApplyStatusEffect(this.statusEffect.ID))
                        {
                            info.message += source == target ?
                                $"{source.Name} {statusEffect.Description} themself with {statusEffect.Name}. [P:0.2]" :
                                $"{source.Name} {statusEffect.Description} {target.Name} with {statusEffect.Name}. [P:0.2]";
                        }
                        else
                        {
                            info.message += $"{source.Name} already has {statusEffect.Name}. [P:0.2]";
                        }
                    }
                    else
                    {
                        info.message += $"{statusEffect.Name} failed. [P:0.2]";
                    }
                }
            }

            if (this.constitutionUp is not null)
                info.message += $"{target.Name}'s {Strings.CharacterStats.Constitution} went up by {RunNumericProperty(this.constitutionUp, source, target.ConstitutionUp)}. [P:0.2]";
            if (this.enduranceUp is not null)
                info.message += $"{target.Name}'s {Strings.CharacterStats.Endurance} went up by {RunNumericProperty(this.enduranceUp, source, target.EnduranceUp)}. [P:0.2]";
            if (this.swiftnessUp is not null)
                info.message += $"{target.Name}'s {Strings.CharacterStats.Swiftness} went up by {RunNumericProperty(this.swiftnessUp, source, target.SwiftnessUp)}. [P:0.2]";
            if (this.strengthUp is not null)
                info.message += $"{target.Name}'s {Strings.CharacterStats.Strength} went up by {RunNumericProperty(this.strengthUp, source, target.StrengthUp)}. [P:0.2]";
            if (this.toughnessUp is not null)
                info.message += $"{target.Name}'s {Strings.CharacterStats.Toughness} went up by {RunNumericProperty(this.toughnessUp, source, target.ToughnessUp)}. [P:0.2]";
            if (this.willUp is not null)
                info.message += $"{target.Name}'s {Strings.CharacterStats.Will} went up by {RunNumericProperty(this.willUp, source, target.WillUp)}. [P:0.2]";
            if (this.fortuneUp is not null)
                info.message += $"{target.Name}'s {Strings.CharacterStats.Fortune} went up by {RunNumericProperty(this.fortuneUp, source, target.FortuneUp)}. [P:0.2]";

            info.targetWasKilled = !targetDeadPrevious && target.Dead;
            info.targetWasRevived = targetDeadPrevious && !target.Dead;

            return info;
        }

        public string TextBreakdown(CharacterStats source)
        {
            string text = string.Empty;

            if (this.Damage is not null)
                text += TextBreakdownHelper(source, this.Damage, Strings.CharacterStats.Damage, Colors.Red);

            if (this.DirectDamage is not null)
                text += TextBreakdownHelper(source, this.DirectDamage, Strings.CharacterStats.Damage, Colors.Red);

            if (this.CriticalDamage is not null)
                text += TextBreakdownHelper(source, this.CriticalDamage, Strings.CharacterStats.CriticalDamage, Colors.Yellow);

            if (this.PsychicDamage is not null)
                text += TextBreakdownHelper(source, this.PsychicDamage, Strings.CharacterStats.PsychicDamage + '\n', Colors.Magenta);

            if (this.Heal is not null)
                text += TextBreakdownHelper(source, this.Heal, Strings.CharacterStats.Heal, Colors.Green);

            if (this.Recover is not null)
                text += TextBreakdownHelper(source, this.Recover, Strings.CharacterStats.Recover, Colors.Cyan);

            if (this.Exert is not null)
                text += TextBreakdownHelper(source, this.Exert, Strings.CharacterStats.Exert, Colors.Blue);

            if (this.ResetHealth)
                text += $"Fill {Strings.CharacterStats.Health} ({source.MaxHealth})\n";

            if (this.ResetStamina)
                text += $"Fill {Strings.CharacterStats.Stamina} ({source.MaxStamina})\n";

            if (this.ConstitutionUp is not null)
                text += TextBreakdownHelper(source, this.ConstitutionUp, $"{Strings.CharacterStats.Constitution} Up", Colors.Orange);

            if (this.EnduranceUp is not null)
                text += TextBreakdownHelper(source, this.EnduranceUp, $"{Strings.CharacterStats.Endurance} Up", Colors.Orange);

            if (this.SwiftnessUp is not null)
                text += TextBreakdownHelper(source, this.SwiftnessUp, $"{Strings.CharacterStats.Swiftness} Up", Colors.Orange);

            if (this.StrengthUp is not null)
                text += TextBreakdownHelper(source, this.StrengthUp, $"{Strings.CharacterStats.Strength} Up", Colors.Orange);

            if (this.ToughnessUp is not null)
                text += TextBreakdownHelper(source, this.ToughnessUp, $"{Strings.CharacterStats.Toughness} Up", Colors.Orange);

            if (this.WillUp is not null)
                text += TextBreakdownHelper(source, this.WillUp, $"{Strings.CharacterStats.Will} Up", Colors.Orange);

            if (this.FortuneUp is not null)
                text += TextBreakdownHelper(source, this.FortuneUp, $"{Strings.CharacterStats.Fortune} Up", Colors.Orange);

            if (this.StatusEffect is not null)
            {
                StatusEffect effect = this.StatusEffect.Get();
                text += $"{effect?.Name}";
                if (this.StatusEffect.successRate < 100)
                    text += $" <color=#{ColorUtility.ToHtmlStringRGBA(Colors.Cyan)}>{this.StatusEffect.successRate}%+{source.Luck}{Strings.CharacterStats.LuckSymbol} chance</color>";
                if (!effect.Singleton)
                    text += ", stackable";
                text += "\n";
            }

            if (this.SuccessRate < 100)
                text += SuccessRateHelper(source);

            return text;
        }
        #endregion

        #region Private Methods
        private int RunNumericProperty(NumericProperty numericProperty, CharacterStats source, CalculatedAction targetAction)
        {
            int value = numericProperty.Calculate(source);

            int result = targetAction(value);

            return result;
        }

        private string TextBreakdownHelper(CharacterStats source, NumericProperty actionProperty, string statsActionName, Color32 color)
        {
            string text = $"{statsActionName} {actionProperty.Number.TextBreakdown(source)}";
            foreach (BF2D.Enums.CharacterStatsProperty modifier in actionProperty.Modifiers)
            {
                text += $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>+{source.GetStatsProperty(modifier)}{Strings.CharacterStats.GetStatsPropertySymbol(modifier)}</color>";
            }
            text += "\n";
            return text;
        }

        private string SuccessRateHelper(CharacterStats source)
        {
            if (this.SuccessRate < 0)
                return $"Always Fails";

            return $"Success Rate <color=#{ColorUtility.ToHtmlStringRGBA(Colors.Cyan)}>{this.SuccessRate}%+{source.Luck}{Strings.CharacterStats.LuckSymbol}</color>\n";
        }
        #endregion
    }
}