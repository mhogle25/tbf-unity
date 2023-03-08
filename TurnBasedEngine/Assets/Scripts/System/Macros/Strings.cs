using BF2D.Enums;
using UnityEngine.Animations;
using System;

namespace BF2D.Game
{
    public static class Strings
    {
        public static class System
        {
            public const string Default = "default";
        }

        public static class Game
        {
            public const string Currency = "USD";
            public const string Bag = "Bag";
            public const string StatusEffect = "Status Effect";
        }

        public static class CharacterStats
        {
            public const string Damage = "Damage";
            public const string CriticalDamage = "Critical";
            public const string PsychicDamage = "Psychic Damage";
            public const string Heal = "Heal";
            public const string Recover = "Recover";
            public const string Exert = "Exert";

            public const string Health = "Health";
            public const string MaxHealth = "Max Health";
            public const string Stamina = "Stamina";
            public const string MaxStamina = "Max Stamina";

            public const string Speed = "Speed";
            public const string Attack = "Attack";
            public const string Defense = "Defense";
            public const string Focus = "Focus";
            public const string Luck = "Luck";

            public const string Constitution = "Constitution";
            public const string Endurance = "Endurance";
            public const string Swiftness = "Swiftness";
            public const string Strength = "Strength";
            public const string Toughness = "Toughness";
            public const string Will = "Will";
            public const string Fortune = "Fortune";

            public const string CritMultiplier = "Critical Hit Multiplier";
            public const string CritChance = "Critical Hit Chance";

            public const char SpeedSymbol = 'ª';
            public const char AttackSymbol = '¨';
            public const char DefenseSymbol = '©';
            public const char FocusSymbol = '«';
            public const char LuckSymbol = '¬';
            public const char MaxHealthSymbol = '§';
            public const char MaxStaminaSymbol = '®';

            public static char GetStatsPropertySymbol(CharacterStatsProperty property) 
            {
                return property switch
                {
                    CharacterStatsProperty.Speed => SpeedSymbol,
                    CharacterStatsProperty.Attack => AttackSymbol,
                    CharacterStatsProperty.Defense => DefenseSymbol,
                    CharacterStatsProperty.Focus => FocusSymbol,
                    CharacterStatsProperty.Luck => LuckSymbol,
                    CharacterStatsProperty.MaxHealth => MaxHealthSymbol,
                    CharacterStatsProperty.MaxStamina => MaxStaminaSymbol,
                    _ => ' '
                };
            }

        }

        public static class Animation
        {
            public const string Idle = "idle";
            public const string Attack = "attack";
            public const string Flashing = "flashing";
            public const string Damaged = "damaged";
        }

        public static string IntToStringSigned(int value)
        {
            if (value > 0)
                return $"+{value}";


            if (value < 0)
                return $"-{Math.Abs(value)}";

            return null;
        }
    }
}