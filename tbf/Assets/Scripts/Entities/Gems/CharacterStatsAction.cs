using UnityEngine;
using System;
using Newtonsoft.Json;
using BF2D.Enums;
using BF2D.Game.Enums;
using System.Collections.Generic;
using BF2D.Utilities;

namespace BF2D.Game.Actions
{
    /// <summary>
    /// Gem
    /// </summary>
    [Serializable]
    public class CharacterStatsAction : Entity, IUtilityEntity
    {
        public struct Specs
        {
            public NumRandInt repeat;
            public AuraType? hasAura;
            public NumRandInt successRateModifier;
            public NumRandInt critChanceModifier;
        }

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
                    return $"But {this.target.Name} was not affected. {Strings.DialogTextbox.PAUSE_BREIF}";
                return message;
            }
        }

        [JsonProperty] private readonly string spriteID = string.Empty;
        [JsonProperty] private readonly int successRate = 100;
        [JsonProperty] private readonly CombatAlignment? alignment = null;

        [JsonProperty] private readonly NumericProperty damage = null;
        [JsonProperty] private readonly NumericProperty directDamage = null;
        [JsonProperty] private readonly NumericProperty criticalDamage = null;
        [JsonProperty] private readonly NumericProperty psychicDamage = null;
        [JsonProperty] private readonly NumericProperty heal = null;
        [JsonProperty] private readonly NumericProperty recover = null;
        [JsonProperty] private readonly NumericProperty exert = null;
        [JsonProperty] private readonly bool resetHealth = false;
        [JsonProperty] private readonly bool resetStamina = false;
        [JsonProperty] private readonly StatusEffectProperty statusEffect = null;

        // Stats Up
        [JsonProperty] private readonly NumericProperty constitutionUp = null;
        [JsonProperty] private readonly NumericProperty enduranceUp = null;
        [JsonProperty] private readonly NumericProperty swiftnessUp = null;
        [JsonProperty] private readonly NumericProperty strengthUp = null;
        [JsonProperty] private readonly NumericProperty toughnessUp = null;
        [JsonProperty] private readonly NumericProperty willUp = null;
        [JsonProperty] private readonly NumericProperty fortuneUp = null;

        [JsonIgnore] public override string ID { get => this.id; set => this.id = value; }
        [JsonIgnore] private string id = string.Empty;
        [JsonIgnore] public string SpriteID => this.spriteID;
        [JsonIgnore] public int SuccessRate => this.successRate;
        [JsonIgnore] public CombatAlignment Alignment
        {
            get
            {
                if (this.alignment is not null)
                    return this.alignment.GetValueOrDefault();

                int offensePoints = 0;
                int defensePoints = 0;
                int neutralPoints = 0;

                offensePoints += this.damage is not null ? 1 : 0;
                offensePoints += this.directDamage is not null ? 1 : 0;
                offensePoints += this.criticalDamage is not null ? 1 : 0;
                offensePoints += this.psychicDamage is not null ? 1 : 0;
                defensePoints += this.heal is not null ? 1 : 0;
                defensePoints += this.recover is not null ? 1 : 0;
                offensePoints += this.exert is not null ? 1 : 0;
                defensePoints += this.resetHealth ? 1 : 0;
                defensePoints += this.resetStamina ? 1 : 0;

                switch (this.statusEffect?.Get().Alignment)
                {
                    case CombatAlignment.Offense: offensePoints++; break;
                    case CombatAlignment.Defense: defensePoints++; break;
                    case CombatAlignment.Neutral: neutralPoints++; break;
                }

                defensePoints += this.constitutionUp is not null ? 1 : 0;
                neutralPoints += this.enduranceUp is not null ? 1 : 0;
                neutralPoints += this.swiftnessUp is not null ? 1 : 0;
                offensePoints += this.strengthUp is not null ? 1 : 0;
                defensePoints += this.toughnessUp is not null ? 1 : 0;
                neutralPoints += this.willUp is not null ? 1 : 0;
                neutralPoints += this.fortuneUp is not null ? 1 : 0;

                return CombatAlignmentSelector.Calculate(offensePoints, defensePoints, neutralPoints);
            }
        }

        [JsonIgnore]
        public bool HasStatsUp => this.constitutionUp is not null || this.enduranceUp is not null || this.swiftnessUp is not null ||
                                  this.strengthUp is not null || this.toughnessUp is not null || this.willUp is not null ||
                                  this.fortuneUp is not null;

        #region Public Methods
        public bool ContainsAura(AuraType aura, Specs specs) => ContainsAura(aura) || aura == specs.hasAura;

        public string GetAnimationKey()
        {
            if (this.criticalDamage is not null || this.psychicDamage is not null || this.directDamage is not null || this.damage is not null)
                return Strings.Animation.DAMAGED_ID;

            return Strings.Animation.FLASHING_ID;
        }

        public Info Run(CharacterStats source, CharacterStats target, Specs specs)
        {
            Info info = new()
            {
                target = target
            };

            if (!Probability.Roll(source, this.successRate + specs.successRateModifier.Calculate(source)))
            {
                info.failed = true;
                return info;
            }

            bool targetDeadPrevious = target.Dead;

            int repeatRoll = specs.repeat.Calculate(source);
            int repeatCount = repeatRoll < 1 ? 1 : repeatRoll;
            for (int i = 0; i < repeatCount; i++)
            {
                if (this.resetHealth)
                {
                    info.message += $"{target.Name}'s {Strings.Character.HEALTH} went up to full. {Strings.DialogTextbox.PAUSE_BREIF}";
                    target.ResetHealth();
                }

                if (this.resetStamina)
                {
                    info.message += $"{target.Name}'s {Strings.Character.STAMINA} went up to full. {Strings.DialogTextbox.PAUSE_BREIF}";
                    target.ResetStamina();
                }

                bool deathGem = ContainsAura(AuraType.Death, specs);
                bool deathTarget = target.ContainsAura(AuraType.Death);

                if (this.damage is not null)
                    if (deathGem && deathTarget)
                        info.message += RnHealHelper(this.damage, source, target);
                    else
                        info.message += RnDamageHelper(this.damage, target.Damage, source, target, true, specs);

                if (this.directDamage is not null)
                    if (deathGem && deathTarget)
                        info.message += RnHealHelper(this.directDamage, source, target);
                    else 
                        info.message += RnDamageHelper(this.directDamage, target.DirectDamage, source, target, true, specs);

                if (this.psychicDamage is not null)
                    if (deathGem && deathTarget)
                        info.message += RnHealHelper(this.psychicDamage, source, target);
                    else 
                        info.message += RnDamageHelper(this.psychicDamage, target.PsychicDamage, source, target, true, specs);

                if (this.criticalDamage is not null)
                    if (deathGem && deathTarget)
                        info.message += RnHealHelper(this.criticalDamage, source, target);
                    else
                        info.message += RnCriticalDamageHelper(this.criticalDamage, source, target);

                if (this.heal is not null)
                    if ((deathGem && deathTarget) || (!deathGem && !deathTarget))
                        info.message += RnHealHelper(this.heal, source, target);
                    else
                        info.message += RnDamageHelper(this.heal, target.Damage, source, target, false, specs);

                if (this.recover is not null)
                    info.message += $"{target.Name} recovered {this.recover.Run(source, target.Recover)} {Strings.Character.STAMINA.ToLower()}. {Strings.DialogTextbox.PAUSE_BREIF}";

                if (this.exert is not null)
                    info.message += $"{target.Name} exerted {this.exert.Run(source, target.Exert)} {Strings.Character.STAMINA.ToLower()}. {Strings.DialogTextbox.PAUSE_BREIF}";

                if (this.statusEffect is not null)
                    info.message += this.statusEffect.Run(source, target);

                if (this.constitutionUp is not null)
                    info.message += $"{target.Name}'s {Strings.Character.CONSTITUTION} went up by {this.constitutionUp.Run(source, target.ConstitutionUp)}. {Strings.DialogTextbox.PAUSE_BREIF}";

                if (this.enduranceUp is not null)
                    info.message += $"{target.Name}'s {Strings.Character.ENDURANCE} went up by {this.enduranceUp.Run(source, target.EnduranceUp)}. {Strings.DialogTextbox.PAUSE_BREIF}";

                if (this.swiftnessUp is not null)
                    info.message += $"{target.Name}'s {Strings.Character.SWIFTNESS} went up by {this.swiftnessUp.Run(source, target.SwiftnessUp)}. {Strings.DialogTextbox.PAUSE_BREIF}";

                if (this.strengthUp is not null)
                    info.message += $"{target.Name}'s {Strings.Character.STRENGTH} went up by {this.strengthUp.Run(source, target.StrengthUp)}. {Strings.DialogTextbox.PAUSE_BREIF}";

                if (this.toughnessUp is not null)
                    info.message += $"{target.Name}'s {Strings.Character.TOUGHNESS} went up by {this.toughnessUp.Run(source, target.ToughnessUp)}. {Strings.DialogTextbox.PAUSE_BREIF}";

                if (this.willUp is not null)
                    info.message += $"{target.Name}'s {Strings.Character.WILL} went up by {this.willUp.Run(source, target.WillUp)}. {Strings.DialogTextbox.PAUSE_BREIF}";

                if (this.fortuneUp is not null)
                    info.message += $"{target.Name}'s {Strings.Character.FORTUNE} went up by {this.fortuneUp.Run(source, target.FortuneUp)}. {Strings.DialogTextbox.PAUSE_BREIF}";
            }

            info.targetWasKilled = !targetDeadPrevious && target.Dead;
            info.targetWasRevived = targetDeadPrevious && !target.Dead;

            return info;
        }

        public string TextBreakdown(CharacterStats source, Specs specs)
        {
            string text = string.Empty;

            if (this.damage is not null)
                text += TbNumericPropertyHelper(this.damage, source, Strings.Character.DAMAGE, Colors.Red);

            if (this.directDamage is not null)
                text += TbNumericPropertyHelper(this.directDamage, source, Strings.Character.DAMAGE, Colors.Red);

            if (this.criticalDamage is not null)
                text += TbNumericPropertyHelper(this.criticalDamage, source, Strings.Character.CRITICAL_DAMAGE, Colors.Yellow);

            if (this.psychicDamage is not null)
                text += TbNumericPropertyHelper(this.psychicDamage, source, Strings.Character.PSYCHIC_DAMAGE, Colors.Magenta);

            if (this.heal is not null)
                text += TbNumericPropertyHelper(this.heal, source, Strings.Character.HEAL, Colors.Green);

            if (this.recover is not null)
                text += TbNumericPropertyHelper(this.recover, source, Strings.Character.RECOVER, Colors.Cyan);

            if (this.exert is not null)
                text += TbNumericPropertyHelper(this.exert, source, Strings.Character.EXERT, Colors.Blue);

            if (this.resetHealth)
                text += $"Fully Restore {Strings.Character.HEALTH}\n";

            if (this.resetStamina)
                text += $"Fully Restore {Strings.Character.STAMINA}\n";

            if (this.constitutionUp is not null)
                text += TbNumericPropertyHelper(this.constitutionUp, source, $"{Strings.Character.CONSTITUTION} Up", Colors.Orange);

            if (this.enduranceUp is not null)
                text += TbNumericPropertyHelper(this.enduranceUp, source, $"{Strings.Character.ENDURANCE} Up", Colors.Orange);

            if (this.swiftnessUp is not null)
                text += TbNumericPropertyHelper(this.swiftnessUp, source, $"{Strings.Character.SWIFTNESS} Up", Colors.Orange);

            if (this.strengthUp is not null)
                text += TbNumericPropertyHelper(this.strengthUp, source, $"{Strings.Character.STRENGTH} Up", Colors.Orange);

            if (this.toughnessUp is not null)
                text += TbNumericPropertyHelper(this.toughnessUp, source, $"{Strings.Character.TOUGHNESS} Up", Colors.Orange);

            if (this.willUp is not null)
                text += TbNumericPropertyHelper(this.willUp, source, $"{Strings.Character.WILL} Up", Colors.Orange);

            if (this.fortuneUp is not null)
                text += TbNumericPropertyHelper(this.fortuneUp, source, $"{Strings.Character.FORTUNE} Up", Colors.Orange);

            if (this.statusEffect is not null)
                text += this.statusEffect.TextBreakdownSimple(source);

            if (this.SuccessRate < 100)
                text += TbSuccessRateHelper(source, specs);

            List<string> specAdditions = new();
            if (specs.repeat.Value is not 1 || !string.IsNullOrEmpty(specs.repeat.Expression))
                specAdditions.Add($"x{specs.repeat.TextBreakdown(source)}");

            if (specs.hasAura is not null)
                specAdditions.Add($"Has {specs.hasAura}");

            if (specs.critChanceModifier.Value is not 0 || !string.IsNullOrEmpty(specs.critChanceModifier.Expression))
                specAdditions.Add($"+{specs.critChanceModifier.TextBreakdown(source)}% Crit Chance");

            if (specAdditions.Count > 0)
                text += $"{string.Join(", ", specAdditions)}\n";

            return text;
        }
        #endregion

        #region Private Methods
        private string RnDamageHelper(NumericProperty numericProperty, CalculatedAction action, CharacterStats source, CharacterStats target, bool critEnabled, Specs specs)
        {
            string message = string.Empty;
            if (Probability.Roll(source, source.CurrentJob.CritChance + specs.critChanceModifier.Calculate(source)) && !target.CritImmune && critEnabled)
                message += RnCriticalDamageHelper(numericProperty, source, target);
            else
                message += $"{target.Name} took {numericProperty.Run(source, action)} {Strings.Character.DAMAGE.ToLower()}. {Strings.DialogTextbox.PAUSE_BREIF}";
            return message;
        }

        private string RnCriticalDamageHelper(NumericProperty numericProperty, CharacterStats source, CharacterStats target)
        {
            return $"{Strings.Character.CRITICAL_DAMAGE}.{Strings.DialogTextbox.PAUSE_BREIF} {target.Name} took {numericProperty.Run(source, target.CriticalDamage)} {Strings.Character.DAMAGE.ToLower()}.{Strings.DialogTextbox.PAUSE_BREIF}";
        }

        private string RnHealHelper(NumericProperty numericProperty, CharacterStats source, CharacterStats target)
        {
            return $"{target.Name} gained {numericProperty.Run(source, target.Heal)} {Strings.Character.HEALTH.ToLower()}. {Strings.DialogTextbox.PAUSE_BREIF}";
        }

        private string TbNumericPropertyHelper(NumericProperty numericProperty, CharacterStats source, string statsActionName, Color32 color)
        {
            string numericPropertyText = numericProperty.Number.TextBreakdown(source);
            string text = $"{statsActionName} {numericPropertyText}";

            if (source is not null)
                foreach (CharacterStatsProperty modifier in numericProperty.Modifiers)
                    text += $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{source.GetModifierText(modifier)}</color>";

            text += '\n';
            return text;
        }

        private string TbSuccessRateHelper(CharacterStats source, Specs specs)
        {
            if (this.SuccessRate < 0)
                return $"Always Fails";

            string successRateModifier = source is null ? null : $"+{specs.successRateModifier.TextBreakdown(source)}%{source.GetModifierText(CharacterStatsProperty.Luck)}";
            return $"Success Rate <color=#{ColorUtility.ToHtmlStringRGBA(Colors.Cyan)}>{this.SuccessRate}{successRateModifier}</color>\n";
        }
        #endregion
    }
}