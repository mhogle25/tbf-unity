using Newtonsoft.Json;
using System;
using BF2D.Game.Enums;
using UnityEngine;
using BF2D.Utilities;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class NumericProperty
    {
        [JsonIgnore] public NumRandInt Number { get => this.number; }
        [JsonProperty] private readonly NumRandInt number = new(0);

        [JsonIgnore] public CharacterStatsProperty[] Modifiers { get => this.modifiers; }
        [JsonProperty] private readonly CharacterStatsProperty[] modifiers = { };

        public int Run(CharacterStats source, CalculatedAction targetAction)
        {
            int value = this.Calculate(source);

            int result = targetAction(value);

            return result;
        }

        public int Calculate(CharacterStats character)
        {
            int total = this.Number.Calculate(character);

            if (this.modifiers is null)
                return total;

            foreach (CharacterStatsProperty property in this.modifiers)
                total += character.GetStatsProperty(property);

            return total;
        }

        public string TextBreakdown(CharacterStats source, Color32 color)
        {
            string text = this.Number.TextBreakdown(source);

            if (source is not null)
                foreach (CharacterStatsProperty modifier in this.Modifiers)
                    text += $"{source.GetModifierText(modifier)}";
            return text.Colorize(color);
        }
    }
}