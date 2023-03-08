using BF2D.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

        [JsonProperty] private readonly float initLevelUpAmount = 10f;
        [JsonProperty] private readonly float levelUpRate = 1.5f;

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

        [JsonProperty] private int experienceAwardDivisor = 3;

        public int GetLevel(int experience)
        {
            int level = 1;
            float levelUpAmount = this.initLevelUpAmount;

            while (experience > levelUpAmount)
            {
                level++;
                levelUpAmount += levelUpAmount*this.levelUpRate;
            }

            return level;
        }

        public void ForEachLevel(int experience, Action<int> action)
        {
            int level = 1;
            float levelUpAmount = this.initLevelUpAmount;

            while (experience > levelUpAmount)
            {
                action?.Invoke(level);
                level++;
                levelUpAmount += levelUpAmount*this.levelUpRate;
            }
        }

        public int GetMaxHealthModifier(int experience)
        {
            return Calculate(experience, this.maxHealthRate, (level) =>
            {
                LevelUpEvent levelUpEvent = GetLevelUpEvent(level);
                if (levelUpEvent is null)
                    return 0;
                return levelUpEvent.maxHealth;
            });
        }

        public int GetMaxStaminaModifier(int experience)
        {
            return Calculate(experience, this.maxStaminaRate, (level) =>
            {
                LevelUpEvent levelUpEvent = GetLevelUpEvent(level);
                if (levelUpEvent is null)
                    return 0;
                return levelUpEvent.maxStamina;
            });
        }

        public int GetSpeedModifier(int experience)
        {
            return Calculate(experience, this.speedRate, (level) =>
            {
                LevelUpEvent levelUpEvent = GetLevelUpEvent(level);
                if (levelUpEvent is null)
                    return 0;
                return levelUpEvent.speed;
            });
        }

        public int GetAttackModifier(int experience)
        {
            return Calculate(experience, this.attackRate, (level) =>
            {
                LevelUpEvent levelUpEvent = GetLevelUpEvent(level);
                if (levelUpEvent is null)
                    return 0;
                return levelUpEvent.attack;
            });
        }

        public int GetDefenseModifier(int experience)
        {
            return Calculate(experience, this.defenseRate, (level) =>
            {
                LevelUpEvent levelUpEvent = GetLevelUpEvent(level);
                if (levelUpEvent is null)
                    return 0;
                return levelUpEvent.defense;
            });
        }

        public int GetFocusModifier(int experience)
        {
            return Calculate(experience, this.focusRate, (level) =>
            {
                LevelUpEvent levelUpEvent = GetLevelUpEvent(level);
                if (levelUpEvent is null)
                    return 0;
                return levelUpEvent.focus;
            });
        }

        public int GetLuckModifier(int experience)
        {
            return Calculate(experience, this.luckRate, (level) =>
            {
                LevelUpEvent levelUpEvent = GetLevelUpEvent(level);
                if (levelUpEvent is null)
                    return 0;
                return levelUpEvent.luck;
            });
        }

        public int GetCritMultiplier(int experience)
        {
            return Numbers.BaseCritMultiplier + Calculate(experience, this.critMultiplierRate, (level) =>
            {
                LevelUpEvent levelUpEvent = GetLevelUpEvent(level);
                if (levelUpEvent is null)
                    return 0;
                return levelUpEvent.critMultiplier;
            });
        }

        public int GetCritChance(int experience)
        {
            return Numbers.BaseCritChance + Calculate(experience, this.critChanceRate, (level) =>
            {
                LevelUpEvent levelUpEvent = GetLevelUpEvent(level);
                if (levelUpEvent is null)
                    return 0;
                return levelUpEvent.critChance;
            });
        }

        public int GetExperienceAward(int experience)
        {
            int modifier = 0;
            LevelUpEvent levelup = GetLevelUpEvent(GetLevel(experience));
            if (levelup is not null)
                modifier = levelup.experienceAwardDivisor;

            return experience / (this.experienceAwardDivisor + modifier);
        }

        public string GetLevelUpMessage(int previousExperience, int currentExperience)
        {

            string statsMessage = string.Empty;
            statsMessage += GetLevelUpDialogHelper(Strings.CharacterStats.MaxHealth + Strings.CharacterStats.MaxHealthSymbol, GetMaxHealthModifier(previousExperience), GetMaxHealthModifier(currentExperience));
            statsMessage += GetLevelUpDialogHelper(Strings.CharacterStats.MaxStamina + Strings.CharacterStats.MaxStaminaSymbol, GetMaxStaminaModifier(previousExperience), GetMaxStaminaModifier(currentExperience));
            statsMessage += GetLevelUpDialogHelper(Strings.CharacterStats.Speed + Strings.CharacterStats.SpeedSymbol, GetSpeedModifier(previousExperience), GetSpeedModifier(currentExperience));
            statsMessage += GetLevelUpDialogHelper(Strings.CharacterStats.Attack + Strings.CharacterStats.AttackSymbol, GetAttackModifier(previousExperience), GetAttackModifier(currentExperience));
            statsMessage += GetLevelUpDialogHelper(Strings.CharacterStats.Defense + Strings.CharacterStats.DefenseSymbol, GetDefenseModifier(previousExperience), GetDefenseModifier(currentExperience));
            statsMessage += GetLevelUpDialogHelper(Strings.CharacterStats.Focus + Strings.CharacterStats.FocusSymbol, GetFocusModifier(previousExperience), GetFocusModifier(currentExperience));
            statsMessage += GetLevelUpDialogHelper(Strings.CharacterStats.Luck + Strings.CharacterStats.LuckSymbol, GetLuckModifier(previousExperience), GetLuckModifier(currentExperience));
            statsMessage += GetLevelUpDialogHelper(Strings.CharacterStats.CritMultiplier.ToLower(), GetCritMultiplier(previousExperience), GetCritMultiplier(currentExperience));
            statsMessage += GetLevelUpDialogHelper(Strings.CharacterStats.CritChance.ToLower(), GetCritChance(previousExperience), GetCritChance(currentExperience));

            return statsMessage;
        }

        private string GetLevelUpDialogHelper(string label, int previousValue, int currentValue)
        {
            string modified = string.Empty;
            if (previousValue == currentValue)
                return string.Empty;
            if (previousValue < currentValue)
                modified = "increased";
            if (previousValue > currentValue)
                modified = "decreased";
            return $"[I:0]'s {label} {modified} by {currentValue - previousValue}. [P:0.2]";
        }

        private int Calculate(int experience, Rate rate, Func<int, int> forEach)
        {
            int value = 0;
            int skipCount = 0;
            ForEachLevel(experience, (level) =>
            {
                value += forEach.Invoke(level);

                if (rate.skip > skipCount)
                {
                    skipCount++;
                    return;
                }
                skipCount = 0;
                value += rate.amount;
            });

            return value;
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

        private LevelUpEvent GetLevelUpEvent(int level)
        {
            return this.levelUpEvents.Find((levelUpEvent) => levelUpEvent.level == level);
        }

        #endregion
    }
}