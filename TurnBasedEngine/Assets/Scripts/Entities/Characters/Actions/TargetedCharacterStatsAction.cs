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
            string text = "-\n";

            if (this.Gem.Damage != null)
            {
                text += TextBreakdownHelper(character, this.Gem.Damage, BF2D.Game.Strings.CharacterStats.Damage, BF2D.Game.Colors.red);
            }

            if (this.Gem.DirectDamage != null)
            {
                text += TextBreakdownHelper(character, this.Gem.DirectDamage, BF2D.Game.Strings.CharacterStats.Damage, BF2D.Game.Colors.red);
            }

            if (this.Gem.CriticalDamage != null)
            {
                text += TextBreakdownHelper(character, this.Gem.CriticalDamage, BF2D.Game.Strings.CharacterStats.CriticalDamage, BF2D.Game.Colors.yellow);
            }

            if (this.Gem.PsychicDamage != null)
            {
                text += TextBreakdownHelper(character, this.Gem.PsychicDamage, BF2D.Game.Strings.CharacterStats.PsychicDamage, BF2D.Game.Colors.magenta);
            }

            if (this.Gem.Heal != null)
            {
                text += TextBreakdownHelper(character, this.Gem.Heal, BF2D.Game.Strings.CharacterStats.Heal, BF2D.Game.Colors.green);
            }

            if (this.Gem.Recover != null)
            {
                text += TextBreakdownHelper(character, this.Gem.Recover, BF2D.Game.Strings.CharacterStats.Recover, BF2D.Game.Colors.cyan);
            }

            if (this.Gem.Exert != null)
            {
                text += TextBreakdownHelper(character, this.Gem.Exert, BF2D.Game.Strings.CharacterStats.Exert, BF2D.Game.Colors.blue);
            }

            if (this.Gem.ResetHealth)
            {
                text += $"Fully restore {Strings.CharacterStats.Health} ({character.MaxHealth})\n";
            }

            if (this.Gem.ResetStamina)
            {
                text += $"Fully restore {Strings.CharacterStats.Stamina} ({character.MaxStamina})\n";
            }

            text += "-";

            return text;
        }

        private string TextBreakdownHelper(CharacterStats character, CharacterStatsActionProperty actionProperty, string statsActionName, Color32 color)
        {
            string text = $"{statsActionName} ";
            foreach (CharacterStatsProperty modifier in actionProperty.Modifiers)
            {
                text += $"{actionProperty.Value}<color=#{ColorUtility.ToHtmlStringRGBA(color)}>+{character.GetStatsProperty(modifier)}</color>";
            }
            text += "\n";
            return text;
        }
    }
}
