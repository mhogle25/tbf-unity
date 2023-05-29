using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace BF2D.Game
{
    [Serializable]
    public class Job : Entity
    {

        #region Character Stats
        [Serializable]
        private class Rate
        {
            [JsonProperty] public int skip = 0;
            [JsonProperty] public int amount = 0;

            public void Modify(Rate modification)
            {
                this.skip += modification.skip;
                this.amount += modification.amount;
            }
        }

        [JsonProperty] private readonly long initLevelUpAmount = 3;
        [JsonProperty] private readonly float levelUpRate = 1.6f;
        [JsonProperty] private readonly int experienceAward = 1;

        [JsonProperty]
        private readonly Rate maxHealthRate = new()
        {
            skip = 0,
            amount = 1,
        };

        [JsonProperty]
        private readonly Rate maxStaminaRate = new()
        {
            skip = 0,
            amount = 1,
        };

        [JsonProperty]
        private readonly Rate speedRate = new()
        {
            skip = 0,
            amount = 1,
        };

        [JsonProperty]
        private readonly Rate attackRate = new()
        {
            skip = 0,
            amount = 1,
        };

        [JsonProperty]
        private readonly Rate defenseRate = new()
        {
            skip = 0,
            amount = 1,
        };

        [JsonProperty]
        private readonly Rate focusRate = new()
        {
            skip = 0,
            amount = 1,
        };

        [JsonProperty]
        private readonly Rate luckRate = new()
        {
            skip = 0,
            amount = 1,
        };

        [JsonProperty]
        private readonly Rate critMultiplierRate = new()
        {
            skip = 0,
            amount = 1,
        };

        [JsonProperty]
        private readonly Rate critChanceRate = new()
        {
            skip = 0,
            amount = 1,
        };

        public long ExperienceAward => this.experienceAward;

        public int GetMaxHealthModifier(int level)
        {
            return Calculate(level, this.maxHealthRate, levelUpEvent => levelUpEvent.maxHealth);
        }

        public int GetMaxStaminaModifier(int level)
        {
            return Calculate(level, this.maxStaminaRate, levelUpEvent => levelUpEvent.maxStamina);
        }

        public int GetSpeedModifier(int level)
        {
            return Calculate(level, this.speedRate, levelUpEvent => levelUpEvent.speed);
        }

        public int GetAttackModifier(int level)
        {
            return Calculate(level, this.attackRate, levelUpEvent => levelUpEvent.attack);
        }

        public int GetDefenseModifier(int level)
        {
            return Calculate(level, this.defenseRate, levelUpEvent => levelUpEvent.defense);
        }

        public int GetFocusModifier(int level)
        {
            return Calculate(level, this.focusRate, levelUpEvent => levelUpEvent.focus);
        }

        public int GetLuckModifier(int level)
        {
            return Calculate(level, this.luckRate, levelUpEvent => levelUpEvent.luck);
        }

        public int GetCritMultiplier(int level)
        {
            return Numbers.BaseCritMultiplier + Calculate(level, this.critMultiplierRate, levelUpEvent => levelUpEvent.critMultiplier);
        }

        public int GetCritChance(int level)
        {
            return Numbers.BaseCritChance + Calculate(level, this.critChanceRate, levelUpEvent => levelUpEvent.critChance);
        }

        public bool LevelUpdate(ref long experience, ref int level)
        {
            float newLevelUpAmount = this.initLevelUpAmount;

            for (int i = 1; i < level; i++)
                newLevelUpAmount *= this.levelUpRate;

            if (experience > newLevelUpAmount)
            {
                experience = 0;
                level++;
                return true;
            }

            return false;
        }

        public string GetLevelUpMessage(int previousLevel, int currentLevel)
        {
            string statsMessage = string.Empty;
            statsMessage += LevelUpMessageHelper($"{Strings.CharacterStats.MaxHealth}{Strings.CharacterStats.MaxHealthSymbol}", GetMaxHealthModifier(previousLevel), GetMaxHealthModifier(currentLevel));
            statsMessage += LevelUpMessageHelper($"{Strings.CharacterStats.MaxStamina}{Strings.CharacterStats.MaxStaminaSymbol}", GetMaxStaminaModifier(previousLevel), GetMaxStaminaModifier(currentLevel));
            statsMessage += LevelUpMessageHelper($"{Strings.CharacterStats.Speed}{Strings.CharacterStats.SpeedSymbol}", GetSpeedModifier(previousLevel), GetSpeedModifier(currentLevel));
            statsMessage += LevelUpMessageHelper($"{Strings.CharacterStats.Attack}{Strings.CharacterStats.AttackSymbol}", GetAttackModifier(previousLevel), GetAttackModifier(currentLevel));
            statsMessage += LevelUpMessageHelper($"{Strings.CharacterStats.Defense}{Strings.CharacterStats.DefenseSymbol}", GetDefenseModifier(previousLevel), GetDefenseModifier(currentLevel));
            statsMessage += LevelUpMessageHelper($"{Strings.CharacterStats.Focus}{Strings.CharacterStats.FocusSymbol}", GetFocusModifier(previousLevel), GetFocusModifier(currentLevel));
            statsMessage += LevelUpMessageHelper($"{Strings.CharacterStats.Luck}{Strings.CharacterStats.LuckSymbol}", GetLuckModifier(previousLevel), GetLuckModifier(currentLevel));
            statsMessage += LevelUpMessageHelper(Strings.CharacterStats.CritMultiplier.ToLower(), GetCritMultiplier(previousLevel), GetCritMultiplier(currentLevel));
            statsMessage += LevelUpMessageHelper(Strings.CharacterStats.CritChance.ToLower(), GetCritChance(previousLevel), GetCritChance(currentLevel));

            return statsMessage;
        }

        private string LevelUpMessageHelper(string label, long previousValue, long currentValue)
        {
            string modified = string.Empty;
            if (previousValue == currentValue)
                return string.Empty;
            if (previousValue < currentValue)
                modified = "increased";
            if (previousValue > currentValue)
                modified = "decreased";
            return $"[I:0]'s {label} {modified} by {Math.Abs(currentValue - previousValue)}. {Strings.DialogTextbox.BriefPause}";
        }
        #endregion

        #region Utilities
        [Serializable]
        private class LevelUpEvent
        {
            [JsonProperty] public int level = 1;
            [JsonProperty] public int maxHealth = 0;
            [JsonProperty] public int maxStamina = 0;
            [JsonProperty] public int speed = 0;
            [JsonProperty] public int attack = 0;
            [JsonProperty] public int defense = 0;
            [JsonProperty] public int focus = 0;
            [JsonProperty] public int luck = 0;
            [JsonProperty] public int critMultiplier = 0;
            [JsonProperty] public int critChance = 0;

            [JsonProperty] public int experienceAwardDivisor = 0;
        }

        [JsonProperty] private readonly List<LevelUpEvent> levelUpEvents = new();
        [JsonIgnore] private readonly Dictionary<int, List<LevelUpEvent>> levelUpEventsDict = new();

        private int Calculate(int level, Rate rate, Func<LevelUpEvent, int> forEach)
        {
            if (this.levelUpEventsDict.Count < 1)
                LoadLevelUpEvents();

            int value = 0;
            for (int i = 1, skipCount = 0; i <= level; i++, skipCount++)
            {
                if (this.levelUpEventsDict.ContainsKey(i))
                    foreach (LevelUpEvent levelUpEvent in this.levelUpEventsDict[i])
                        value += forEach(levelUpEvent);

                if (skipCount >= rate.skip)
                {
                    skipCount = 0;
                    value += rate.amount;
                }
            }
            return value;
        }

        private void LoadLevelUpEvents()
        {
            this.levelUpEventsDict.Clear();
            foreach (LevelUpEvent levelUpEvent in this.levelUpEvents)
            {
                if (this.levelUpEventsDict.ContainsKey(levelUpEvent.level))
                {
                    this.levelUpEventsDict[levelUpEvent.level].Add(levelUpEvent);
                    continue;
                }
                this.levelUpEventsDict[levelUpEvent.level] = new() { levelUpEvent };
            }
        }
        #endregion
    }
}