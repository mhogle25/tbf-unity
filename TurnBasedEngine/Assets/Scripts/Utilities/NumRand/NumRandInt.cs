using Newtonsoft.Json;
using System;
using BF2D.Utilities;
using BF2D.Enums;
using System.Text.RegularExpressions;

namespace BF2D.Game
{
    [Serializable]
    public class NumRandInt
    {
        [JsonProperty] private readonly string expression = string.Empty;
        [JsonProperty] private readonly int value = 0;

        [JsonIgnore] private readonly NumRand calculator = new();
        [JsonIgnore] private Regex regex = new(@"\s+");

        public NumRandInt(int value)
        {
            this.value = value;
        }

        public int Calculate(CharacterStats source)
        {
            try
            {
                return Calculate(new NumRand.CalcSpecs
                {
                    modifyEveryRandOp = source.Luck,
                    termRegistry = new System.Collections.Generic.Dictionary<string, int>
                    {
                        { Strings.CharacterStats.Speed, source.Speed },
                        { Strings.CharacterStats.Attack, source.Attack },
                        { Strings.CharacterStats.Defense, source.Defense },
                        { Strings.CharacterStats.Focus, source.Focus },
                        { Strings.CharacterStats.Luck, source.Luck },
                        { Strings.CharacterStats.MaxHealth, source.MaxHealth },
                        { Strings.CharacterStats.MaxStamina, source.MaxStamina }
                    }
                });
            }
            catch (Exception x)
            {
                Terminal.IO.LogError(x.ToString());
                return 0;
            }
        }

        public int Calculate(NumRand.CalcSpecs specs)
        {
            return this.calculator.Calculate(this.expression, specs) + this.value;
        }

        public string TextBreakdown(CharacterStats source)
        {
            try
            {
                return TextBreakdown(new NumRand.TextSpecs
                {
                    modifyEveryRandOp = $"+{source.GetStatsPropertyText(CharacterStatsProperty.Luck)}",
                    randModifierColor = Colors.Cyan,
                    termRegistry = new System.Collections.Generic.Dictionary<string, string>
                    {
                        { Strings.CharacterStats.Speed, source.GetStatsPropertyText(CharacterStatsProperty.Speed) },
                        { Strings.CharacterStats.Attack, source.GetStatsPropertyText(CharacterStatsProperty.Attack) },
                        { Strings.CharacterStats.Defense, source.GetStatsPropertyText(CharacterStatsProperty.Defense) },
                        { Strings.CharacterStats.Focus, source.GetStatsPropertyText(CharacterStatsProperty.Focus) },
                        { Strings.CharacterStats.Luck, source.GetStatsPropertyText(CharacterStatsProperty.Luck) },
                        { this.regex.Replace(Strings.CharacterStats.MaxHealth, ""), source.GetStatsPropertyText(CharacterStatsProperty.MaxHealth) },
                        { this.regex.Replace(Strings.CharacterStats.MaxStamina, ""), source.GetStatsPropertyText(CharacterStatsProperty.MaxStamina) }
                    }
                });
            }
            catch (Exception x)
            {
                Terminal.IO.LogError(x.Message);
                return string.Empty;
            }
        }

        public string TextBreakdown(NumRand.TextSpecs specs)
        {
            if (this.value != 0 && string.IsNullOrEmpty(this.expression))
                return $"{this.value}";

            if (this.value != 0)
                return $"({this.calculator.TextBreakdown(this.expression, specs)}){Strings.IntToStringSigned(this.value)}";

            return this.calculator.TextBreakdown(this.expression, specs);
        }
    }
}
