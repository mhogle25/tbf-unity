using Newtonsoft.Json;
using BF2D.Enums;

namespace BF2D.Game.Actions
{
    public class CharacterStatsActionProperty
    {
        [JsonIgnore] public int Value { get { return this.value; } }
        [JsonProperty] private readonly int value = 0;

        [JsonIgnore] public CharacterStatsProperty[] Modifiers { get { return this.modifiers; } }
        [JsonProperty] private readonly CharacterStatsProperty[] modifiers = { };

        public int Calculate(CharacterStats character)
        {
            int total = this.value;
            if (this.modifiers == null)
                return total;

            foreach (CharacterStatsProperty property in this.modifiers)
            {
                total += (int)character.GetStatsProperty(property);
            }
            return total;
        }
    }
}