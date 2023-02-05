using Newtonsoft.Json;
using UnityEngine;
using System;

namespace BF2D.Game
{
    public class JobInfo
    {
        [JsonIgnore] public string ID { get { return this.id; } }
        [JsonProperty] private readonly string id = string.Empty;
        [JsonIgnore] public int Experience { get { return this.experience; } set { this.experience = value; } }
        [JsonProperty] private int experience = 0;

        public Job Get()
        {
            return GameInfo.Instance.GetJob(this.id);
        }

        public int Level
        {
            get
            {
                return Get().GetLevel(this.experience);
            }
        }

        public int MaxHealthModifier
        {
            get
            {
                return Get().GetMaxHealthModifier(this.experience);
            }
        }

        public int MaxStaminaModifier
        {
            get
            {
                return Get().GetMaxStaminaModifier(this.experience);
            }
        }

        public int SpeedModifier
        {
            get
            {
                return Get().GetSpeedModifier(this.experience);
            }
        }

        public int AttackModifier
        {
            get
            {
                return Get().GetAttackModifier(this.experience);
            }
        }

        public int DefenseModifier
        {
            get
            {
                return Get().GetDefenseModifier(this.experience);
            }
        }

        public int FocusModifier
        {
            get
            {
                return Get().GetFocusModifier(this.experience);
            }
        }

        public int LuckModifier
        {
            get
            {
                return Get().GetLuckModifier(this.experience);
            }
        }
    }
}