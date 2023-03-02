using BF2D.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using BF2D.Combat;
using System.Drawing;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class TargetedCharacterStatsAction
    {
        [JsonIgnore] public CharacterTarget Target { get { return this.target; } }
        [JsonProperty] private readonly CharacterTarget target = CharacterTarget.Self;
        [JsonIgnore] public string Description { get { return this.description; } }
        [JsonProperty] private readonly string description = "target";
        [JsonIgnore] public CharacterStatsAction Gem { get { return GameInfo.Instance.GetCharacterStatsAction(this.gemID); } }
        [JsonProperty] private readonly string gemID = string.Empty;

        [JsonIgnore] public CharacterTargetInfo TargetInfo { get { return this.targetInfo; } }
        [JsonIgnore] private readonly CharacterTargetInfo targetInfo = new();

        public string TextBreakdown(CharacterStats character)
        {
            string text = string.Empty;

            if (this.Gem.Damage is not null)
                text += TextBreakdownHelper(character, this.Gem.Damage, Strings.CharacterStats.Damage, Colors.Red);

            if (this.Gem.DirectDamage is not null)
                text += TextBreakdownHelper(character, this.Gem.DirectDamage, Strings.CharacterStats.Damage, Colors.Red);

            if (this.Gem.CriticalDamage is not null)
                text += TextBreakdownHelper(character, this.Gem.CriticalDamage, Strings.CharacterStats.CriticalDamage, Colors.Yellow);

            if (this.Gem.PsychicDamage is not null)
                text += TextBreakdownHelper(character, this.Gem.PsychicDamage, Strings.CharacterStats.PsychicDamage + '\n', Colors.Magenta);

            if (this.Gem.Heal is not null)
                text += TextBreakdownHelper(character, this.Gem.Heal, Strings.CharacterStats.Heal, Colors.Green);

            if (this.Gem.Recover is not null)
                text += TextBreakdownHelper(character, this.Gem.Recover, Strings.CharacterStats.Recover, Colors.Cyan);

            if (this.Gem.Exert is not null)
                text += TextBreakdownHelper(character, this.Gem.Exert, Strings.CharacterStats.Exert, Colors.Blue);

            if (this.Gem.ResetHealth)
                text += $"Fill {Strings.CharacterStats.Health} ({character.MaxHealth})\n";

            if (this.Gem.ResetStamina)
                text += $"Fill {Strings.CharacterStats.Stamina} ({character.MaxStamina})\n";

            if (this.Gem.ConstitutionUp is not null)
                text += TextBreakdownHelper(character, this.Gem.ConstitutionUp, $"{Strings.CharacterStats.Constitution} Up", Colors.Orange);

            if (this.Gem.EnduranceUp is not null)
                text += TextBreakdownHelper(character, this.Gem.EnduranceUp, $"{Strings.CharacterStats.Endurance} Up", Colors.Orange);

            if (this.Gem.SwiftnessUp is not null)
                text += TextBreakdownHelper(character, this.Gem.SwiftnessUp, $"{Strings.CharacterStats.Swiftness} Up", Colors.Orange);

            if (this.Gem.StrengthUp is not null)
                text += TextBreakdownHelper(character, this.Gem.StrengthUp, $"{Strings.CharacterStats.Strength} Up", Colors.Orange);

            if (this.Gem.ToughnessUp is not null)
                text += TextBreakdownHelper(character, this.Gem.ToughnessUp, $"{Strings.CharacterStats.Toughness} Up", Colors.Orange);

            if (this.Gem.WillUp is not null)
                text += TextBreakdownHelper(character, this.Gem.WillUp, $"{Strings.CharacterStats.Will} Up", Colors.Orange);

            if (this.Gem.FortuneUp is not null)
                text += TextBreakdownHelper(character, this.Gem.FortuneUp, $"{Strings.CharacterStats.Fortune} Up", Colors.Orange);

            if (this.Gem.StatusEffect is not null)
            {
                StatusEffect effect = GameInfo.Instance.GetStatusEffect(this.Gem.StatusEffect.id);
                text += $"{effect?.Name}";
                if (this.Gem.StatusEffect.successRate < 100)
                    text += $", <color=#{ColorUtility.ToHtmlStringRGBA(Colors.Cyan)}>{this.Gem.StatusEffect.successRate}%+{character.Luck}{Strings.CharacterStats.LuckSymbol} chance</color>\n";
                else text += '\n';
            }

            if (this.Gem.SuccessRate < 100)
                text += SuccessRateHelper();

            return text;
        }

        private string TextBreakdownHelper(CharacterStats character, CharacterStatsActionProperty actionProperty, string statsActionName, Color32 color)
        {
            string text = $"{statsActionName} {actionProperty.Number.TextBreakdown(character)}";
            foreach (CharacterStatsProperty modifier in actionProperty.Modifiers)
            {
                text += $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>+{character.GetStatsProperty(modifier)}{Strings.CharacterStats.GetStatsPropertySymbol(modifier)}</color>";
            }
            text += "\n";
            return text;
        }

        private string SuccessRateHelper()
        {
            if (this.Gem.SuccessRate < 0)
                return $"Always Fails";

            return $"Success Rate {this.Gem.SuccessRate}%\n";
        }
    }
}
