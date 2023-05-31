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

        [JsonIgnore] public string ID => this.id;
        [JsonProperty] private readonly string id = string.Empty;

        [JsonIgnore] public string Name => Get().Name;

        [JsonIgnore] public string Description => Get().Description;

        [JsonIgnore] public IEnumerable<Enums.AuraType> Auras => Get().Auras;

        [JsonIgnore] public long Experience => this.experience;
        [JsonProperty] private long experience = 0;

        [JsonIgnore] public int Level => this.level;
        [JsonProperty] private int level = 1;

        public Job Get() => GameInfo.Instance.GetJob(this.id);

        public Entity GetEntity() => Get();

        [JsonIgnore] public int MaxHealthModifier => Get().GetMaxHealthModifier(this.Level);
        [JsonIgnore] public int MaxStaminaModifier =>  Get().GetMaxStaminaModifier(this.Level);
        [JsonIgnore] public int SpeedModifier => Get().GetSpeedModifier(this.Level);
        [JsonIgnore] public int AttackModifier => Get().GetAttackModifier(this.Level);
        [JsonIgnore] public int DefenseModifier => Get().GetDefenseModifier(this.Level);
        [JsonIgnore] public int FocusModifier => Get().GetFocusModifier(this.Level);
        [JsonIgnore] public int LuckModifier => Get().GetLuckModifier(this.Level);
        [JsonIgnore] public int CritMultiplier => Get().GetCritMultiplier(this.Level);
        [JsonIgnore] public int CritChance => Get().GetCritChance(this.Level);
        [JsonIgnore] public long ExperienceAward => Get().ExperienceAward;

        public LevelUpInfo GrantExperience(CharacterStats parent, long experience)
        {
            LevelUpInfo info = new();
            int previousLevel = this.Level;
            this.experience += experience;
            info.leveledUp = Get().LevelUpdate(ref this.experience, ref this.level);
            info.parent = parent;
            if (info.leveledUp)
                info.levelUpDialog = new()
                {
                    $"{parent.Name} went from level {previousLevel} to level {this.Level}. {Strings.DialogTextbox.BriefPause}",
                    $"{Get().GetLevelUpMessage(previousLevel, this.Level)}{Strings.DialogTextbox.End}"
                };
            return info;
        }
    }
}