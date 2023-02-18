using Newtonsoft.Json;
using System;

namespace BF2D.Game
{
    [Serializable]
    public class Job : Entity
    {
        [Serializable]
        private struct Modifier
        {
            [JsonProperty] public int skip;
            [JsonProperty] public int amount;
        }

        [JsonProperty] private readonly float initLevelUpAmount = 10f;
        [JsonProperty] private readonly float levelUpRate = 1.5f;
        [JsonProperty] private readonly Modifier healthModifier = new Modifier
        {
            skip = 0,
            amount = 1,
        };

        public int GetLevel(int experience)
        {
            int level = 1;
            float levelUpAmount = this.initLevelUpAmount;

            while (experience > levelUpAmount)
            {
                level++;
                levelUpAmount *= levelUpRate;
            }

            return level;
        }

        public int GetMaxHealthModifier(int experience)
        {
            return 0;
        }

        public int GetMaxStaminaModifier(int experience)
        {
            return 0;
        }

        public int GetSpeedModifier(int experience)
        {
            return 0;
        }

        public int GetAttackModifier(int experience)
        {
            return 0;
        }

        public int GetDefenseModifier(int experience)
        {
            return 0;
        }

        public int GetFocusModifier(int experience)
        {
            return 0;
        }

        public int GetLuckModifier(int experience)
        {
            return 0;
        }

        public int GetCritMultiplier(int experience)
        {
            return 2;
        }

        public int GetCritChance(int experience)
        {
            return 0;
        }

        public int GetExperienceAward(int experience)
        {
            return 0;
        }
    }
}