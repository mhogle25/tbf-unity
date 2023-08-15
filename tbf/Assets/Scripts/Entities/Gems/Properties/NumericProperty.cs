using BF2D.Enums;
using Newtonsoft.Json;
using System;

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

        private int Calculate(CharacterStats character)
        {
            int total = this.Number.Calculate(character);

            if (this.modifiers is null)
                return total;

            foreach (CharacterStatsProperty property in this.modifiers)
                total += character.GetStatsProperty(property);

            return total;
        }
    }
}