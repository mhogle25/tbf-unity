using System;
using BF2D.Game.Enums;

namespace BF2D.Game
{
    public static class Strings
    {
        public static class System
        {
            public const string DEFAULT_ID = "default";
            public const char GENERATED_ID_PREFIX = '_';

            public const char LEFT_ARROW_SYMBOL = '‹';
            public const char RIGHT_ARROW_SYMBOL = '›';
            public const char UP_ARROW_SYMBOL = 'º';
            public const char DOWN_ARROW_SYMBOL = '¹';

            public static bool IsGeneratedID(string id)
            {
                return id[0] == System.GENERATED_ID_PREFIX;
            }

            public static string GenerateID()
            {
                return $"{System.GENERATED_ID_PREFIX}{Guid.NewGuid():N}";
            }
        }

        public static class Game
        {
            public const string CURRENCY = "USD";
            public const string ETHER = "Ether";
            public const string BAG = "Bag";
            public const string STATUS_EFFECT = "Status Effect";
        }

        public static class Character
        {
            public const string DAMAGE = "Damage";
            public const string CRITICAL_DAMAGE = "Critical";
            public const string PSYCHIC_DAMAGE = "Psychic";
            public const string HEAL = "Heal";
            public const string RECOVER = "Recover";
            public const string EXERT = "Exert";

            public const string HEALTH = "Health";
            public const string MAX_HEALTH = "Max Health";
            public const string STAMINA = "Stamina";
            public const string MAX_STAMINA = "Max Stamina";

            public const string SPEED = "Speed";
            public const string ATTACK = "Attack";
            public const string DEFENSE = "Defense";
            public const string FOCUS = "Focus";
            public const string LUCK = "Luck";

            public const string CONSTITUTION = "Constitution";
            public const string ENDURANCE = "Endurance";
            public const string SWIFTNESS = "Swiftness";
            public const string STRENGTH = "Strength";
            public const string TOUGHNESS = "Toughness";
            public const string WILL = "Will";
            public const string FORTUNE = "Fortune";

            public const string CRIT_MULTIPLIER = "Critical Hit Multiplier";
            public const string CRIT_CHANCE = "Critical Hit Chance";

            public const char SPEED_SYMBOL = 'ª';
            public const char ATTACK_SYMBOL = '¨';
            public const char DEFENSE_SYMBOL = '©';
            public const char FOCUS_SYMBOL = '«';
            public const char LUCK_SYMBOL = '¬';
            public const char MAX_HEALTH_SYMBOL = '§';
            public const char MAX_STAMINA_SYMBOL = '®';

            public static char GetStatsPropertySymbol(CharacterStatsProperty property) => property switch
            {
                CharacterStatsProperty.Speed => SPEED_SYMBOL,
                CharacterStatsProperty.Attack => ATTACK_SYMBOL,
                CharacterStatsProperty.Defense => DEFENSE_SYMBOL,
                CharacterStatsProperty.Focus => FOCUS_SYMBOL,
                CharacterStatsProperty.Luck => LUCK_SYMBOL,
                CharacterStatsProperty.MaxHealth => MAX_HEALTH_SYMBOL,
                CharacterStatsProperty.MaxStamina => MAX_STAMINA_SYMBOL,
                _ => throw new ArgumentException("[Strings:GetStatsPropertySymbol] CharacterStatsProperty was invalid.")
            };
        }

        public static class Animation
        {
            public const string IDLE_ID = "idle";
            public const string ATTACK_ID = "attack";
            public const string FLASHING_ID = "flashing";
            public const string DAMAGED_ID = "damaged";
        }

        public static class DialogTextbox
        {
            public const string PAUSE_BREIF = "[P:0.2]";
            public const string PAUSE_STANDARD = "[P:0.5]";
            public const string PAUSE_LONG = "[P:1]";
            public const string END = "[E]";
        }

        public static class UI
        {
            public const string THREAD_MAIN = "main";
            public const string THREAD_SYSTEM = "system";
        }

        public static class Equipment
        {
            public const string ACCESSORY = "Accessory";
            public const string HEAD = "Head";
            public const string TORSO = "Torso";
            public const string LEGS = "Legs";
            public const string HANDS = "Hands";
            public const string FEET = "Feet";

            public const string ACCESSORY_ID = "accessory";
            public const string HEAD_ID = "head";
            public const string TORSO_ID = "torso";
            public const string LEGS_ID = "legs";
            public const string HANDS_ID = "hands";
            public const string FEET_ID = "feet";

            public static string GetType(EquipmentType type) => type switch
            {
                EquipmentType.Accessory => Equipment.ACCESSORY,
                EquipmentType.Head => Equipment.HEAD,
                EquipmentType.Torso => Equipment.TORSO,
                EquipmentType.Legs => Equipment.LEGS,
                EquipmentType.Hands => Equipment.HANDS,
                EquipmentType.Feet => Equipment.FEET,
                _ => throw new ArgumentException("[Strings:Equipment:GetType] The given EquipmentType was null or invalid")
            };

            public static string GetTypeID(EquipmentType type) => type switch
            {
                EquipmentType.Accessory => Equipment.ACCESSORY_ID,
                EquipmentType.Head => Equipment.HEAD_ID,
                EquipmentType.Torso => Equipment.TORSO_ID,
                EquipmentType.Legs => Equipment.LEGS_ID,
                EquipmentType.Hands => Equipment.HANDS_ID,
                EquipmentType.Feet => Equipment.FEET_ID,
                _ => throw new ArgumentException("[Strings:Equipment:GetID] The given EquipmentType was null or invalid")
            };
        }
    }
}