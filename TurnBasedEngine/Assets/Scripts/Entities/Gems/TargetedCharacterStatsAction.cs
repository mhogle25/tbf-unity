using BF2D.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using BF2D.Combat;

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

            if (this.Gem.Damage != null)
            {
                text += TextBreakdownHelper(character, this.Gem.Damage, Strings.CharacterStats.Damage, Colors.Red);
            }

            if (this.Gem.DirectDamage != null)
            {
                text += TextBreakdownHelper(character, this.Gem.DirectDamage, Strings.CharacterStats.Damage, Colors.Red);
            }

            if (this.Gem.CriticalDamage != null)
            {
                text += TextBreakdownHelper(character, this.Gem.CriticalDamage, Strings.CharacterStats.CriticalDamage, Colors.Yellow);
            }

            if (this.Gem.PsychicDamage != null)
            {
                text += TextBreakdownHelper(character, this.Gem.PsychicDamage, Strings.CharacterStats.PsychicDamage, Colors.Magenta);
            }

            if (this.Gem.Heal != null)
            {
                text += TextBreakdownHelper(character, this.Gem.Heal, Strings.CharacterStats.Heal, Colors.Green);
            }

            if (this.Gem.Recover != null)
            {
                text += TextBreakdownHelper(character, this.Gem.Recover, Strings.CharacterStats.Recover, Colors.Cyan);
            }

            if (this.Gem.Exert != null)
            {
                text += TextBreakdownHelper(character, this.Gem.Exert, Strings.CharacterStats.Exert, Colors.Blue);
            }

            if (this.Gem.ResetHealth)
            {
                text += $"Fill {Strings.CharacterStats.Health} ({character.MaxHealth})\n";
            }

            if (this.Gem.ResetStamina)
            {
                text += $"Fill {Strings.CharacterStats.Stamina} ({character.MaxStamina})\n";
            }

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
    }
}
