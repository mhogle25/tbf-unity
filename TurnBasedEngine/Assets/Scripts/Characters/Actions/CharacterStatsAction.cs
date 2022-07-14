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
        [JsonProperty] private CharacterTarget target = CharacterTarget.Self;
        [JsonIgnore] public string Description { get { return this.description; } }
        [JsonProperty] private string description = "target";
        /*
        [JsonIgnore] public InputDirection Direction { get { return this.direction; } }
        [JsonProperty] public InputDirection direction = InputDirection.Right;
        [JsonIgnore] public EffectType Effect { get { return this.effect; } }
        [JsonProperty] private EffectType effect = EffectType.Generic;
        */
        [JsonIgnore] public CharacterStatsActionProperties Properties { get { return this.properties; } }
        [JsonProperty] private CharacterStatsActionProperties properties = new CharacterStatsActionProperties();

        [JsonIgnore] public List<CharacterStats> Targets { get { return this.targets; } }
        [JsonIgnore] private readonly List<CharacterStats> targets = new List<CharacterStats>();

        public void AddTarget(CharacterStats character)
        {
            this.targets.Add(character);
        }

        public List<string> Run(CharacterStats source)
        {
            List<string> dialog = new();
            foreach (CharacterStats target in this.targets)
            {
                string message = string.Empty;
                int i = 0;
                foreach (string s in this.properties.Run(source, target))
                {
                    if (i > 4)
                    {
                        dialog.Add(message);
                    }
                    message += $"{s}\n";
                    i++;
                };
            }
            this.targets.Clear();
            return dialog;
        }

        public string TextBreakdown(CharacterStats character)
        {
            string text = "-\n";

            if (this.Properties.Damage != null)
            {
                text += TextBreakdownHelper(character, this.Properties.Damage, this.Properties.Damage.Modifiers, BF2D.Game.Strings.CharacterStats.Damage, BF2D.Game.Colors.red);
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
