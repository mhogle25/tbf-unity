using Newtonsoft.Json;
using BF2D.Enums;
using BF2D.Utilities;

namespace BF2D.Game.Actions
{
    public class NumericProperty
    {
        [JsonIgnore] public NumRandInt Number { get { return this.number; } }
        [JsonProperty] private readonly NumRandInt number = new(0);

        [JsonIgnore] public CharacterStatsProperty[] Modifiers { get { return this.modifiers; } }
        [JsonProperty] private readonly CharacterStatsProperty[] modifiers = { };

        public int Calculate(CharacterStats character)
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