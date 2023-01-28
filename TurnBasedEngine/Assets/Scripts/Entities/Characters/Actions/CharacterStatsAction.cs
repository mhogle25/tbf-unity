using BF2D.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using BF2D.Combat;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class CharacterStatsAction
    {
        [JsonIgnore] public CharacterTarget Target { get { return this.target; } }
        [JsonProperty] private readonly CharacterTarget target = CharacterTarget.Self;
        [JsonIgnore] public string Description { get { return this.description; } }
        [JsonProperty] private readonly string description = "target";
        [JsonIgnore] public CharacterStatsActionProperties Properties { get { return this.properties; } }
        [JsonProperty] private readonly CharacterStatsActionProperties properties = new();

        [JsonIgnore] public CharacterTargetInfo TargetInfo { get { return this.targetInfo; } }
        [JsonIgnore] private readonly CharacterTargetInfo targetInfo = new();

        public string TextBreakdown(CharacterStats character)
        {
            string text = "-\n";

            if (this.Properties.Damage != null)
            {
                text += TextBreakdownHelper(character, this.Properties.Damage, this.Properties.Damage.Modifiers, BF2D.Game.Strings.CharacterStats.Damage, BF2D.Game.Colors.red);
            }

            if (this.Properties.DirectDamage != null)
            {
                text += TextBreakdownHelper(character, this.Properties.DirectDamage, this.Properties.DirectDamage.Modifiers, BF2D.Game.Strings.CharacterStats.Damage, BF2D.Game.Colors.red);
            }

            if (this.Properties.CriticalDamage != null)
            {
                text += TextBreakdownHelper(character, this.Properties.CriticalDamage, this.Properties.CriticalDamage.Modifiers, BF2D.Game.Strings.CharacterStats.CriticalDamage, BF2D.Game.Colors.yellow);
            }

            if (this.Properties.PsychicDamage != null)
            {
                text += TextBreakdownHelper(character, this.Properties.PsychicDamage, this.Properties.PsychicDamage.Modifiers, BF2D.Game.Strings.CharacterStats.PsychicDamage, BF2D.Game.Colors.magenta);
            }

            if (this.Properties.Heal != null)
            {
                text += TextBreakdownHelper(character, this.Properties.Heal, this.Properties.Heal.Modifiers, BF2D.Game.Strings.CharacterStats.Heal, BF2D.Game.Colors.green);
            }

            if (this.Properties.Recover != null)
            {
                text += TextBreakdownHelper(character, this.Properties.Recover, this.Properties.Recover.Modifiers, BF2D.Game.Strings.CharacterStats.Recover, BF2D.Game.Colors.cyan);
            }

            if (this.Properties.Exert != null)
            {
                text += TextBreakdownHelper(character, this.Properties.Exert, this.Properties.Exert.Modifiers, BF2D.Game.Strings.CharacterStats.Exert, BF2D.Game.Colors.blue);
            }

            if (this.Properties.ResetHealth)
            {
                text += $"Fully restore {Strings.CharacterStats.Health} ({character.MaxHealth})\n";
            }

            if (this.Properties.ResetStamina)
            {
                text += $"Fully restore {Strings.CharacterStats.Stamina} ({character.MaxStamina})\n";
            }

            text += "-";

            return text;
        }

        private string TextBreakdownHelper(CharacterStats character, CharacterStatsActionProperty actionProperty, CharacterStatsProperty[] modifiers, string statsActionName, Color32 color)
        {
            string text = $"{statsActionName} ";
            foreach (CharacterStatsProperty modifier in modifiers)
            {
                text += $"{actionProperty.Value}<color=#{ColorUtility.ToHtmlStringRGBA(color)}>+{character.GetStatsProperty(modifier)}</color>";
            }
            text += "\n";
            return text;
        }
    }
}
