using Newtonsoft.Json;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.TextCore.Text;

namespace BF2D.Game
{
    public class JobInfo : IEntityInfo
    {
        public class LevelUpInfo
        {
            public CharacterStats parent = null;
            public bool leveledUp = false;
            public List<string> levelUpDialog = null;
        }

        [JsonIgnore] public string ID { get { return this.id; } }
        [JsonProperty] private readonly string id = string.Empty;
        [JsonIgnore] public int Experience
        {
            get
            {
                return this.experience;
            }
        }
        [JsonProperty] private int experience = 0;

        public LevelUpInfo GrantExperience(CharacterStats parent, int experience)
        {
            LevelUpInfo info = new();
            int previousExperience = this.Experience;
            this.experience += experience;
            if (Get().GetLevel(previousExperience) < this.Level)
            {
                info.leveledUp = true;
                List<string> levelUpDialog = new();
                levelUpDialog.Add($"{parent.Name} went from level {Get().GetLevel(previousExperience)} to level {Get().GetLevel(this.experience)}. [P:0.2]");
                string levelUpMessage = Get().GetLevelUpMessage(previousExperience, this.experience);
                if (!string.IsNullOrEmpty(levelUpMessage))
                    levelUpDialog.Add(levelUpMessage + "[E]");
                else
                    levelUpDialog[0] += "[E]";
                info.levelUpDialog = levelUpDialog;
                info.parent = parent;
            }
            return info;
        }

        public Job Get()
        {
            return GameInfo.Instance.GetJob(this.id);
        }

        [JsonIgnore] public Entity GetEntity { get { return Get(); } }

        [JsonIgnore]
        public int Level
        {
            get
            {
                return Get().GetLevel(this.experience);
            }
        }

        [JsonIgnore]
        public int MaxHealthModifier
        {
            get
            {
                return Get().GetMaxHealthModifier(this.experience);
            }
        }

        [JsonIgnore]
        public int MaxStaminaModifier
        {
            get
            {
                return Get().GetMaxStaminaModifier(this.experience);
            }
        }

        [JsonIgnore]
        public int SpeedModifier
        {
            get
            {
                return Get().GetSpeedModifier(this.experience);
            }
        }

        [JsonIgnore]
        public int AttackModifier
        {
            get
            {
                return Get().GetAttackModifier(this.experience);
            }
        }

        [JsonIgnore]
        public int DefenseModifier
        {
            get
            {
                return Get().GetDefenseModifier(this.experience);
            }
        }

        [JsonIgnore]
        public int FocusModifier
        {
            get
            {
                return Get().GetFocusModifier(this.experience);
            }
        }

        [JsonIgnore]
        public int LuckModifier
        {
            get
            {
                return Get().GetLuckModifier(this.experience);
            }
        }

        [JsonIgnore]
        public int CritMultiplier
        {
            get
            {
                return Get().GetCritMultiplier(this.experience);
            }
        }

        [JsonIgnore]
        public int CritChance
        {
            get
            {
                return Get().GetCritChance(this.experience);
            }
        }

        [JsonIgnore]
        public int ExperienceAward
        {
            get
            {
                return Get().GetExperienceAward(this.experience);
            }
        }
    }
}