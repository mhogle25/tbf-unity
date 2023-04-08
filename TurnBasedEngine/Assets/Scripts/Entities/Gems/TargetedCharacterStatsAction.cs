using BF2D.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using BF2D.Utilities;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class TargetedCharacterStatsAction
    {
        [JsonIgnore] public CharacterTarget Target { get => this.target; }
        [JsonProperty] private readonly CharacterTarget target = CharacterTarget.Self;
        [JsonIgnore] public string Description { get => this.description.Wash(); }
        [JsonProperty] private readonly string description = "target";
        [JsonIgnore] public CharacterStatsAction Gem { get => GameInfo.Instance.GetGem(this.gemID); }
        [JsonProperty] private readonly string gemID = string.Empty;

        [JsonIgnore] public CharacterTargetInfo TargetInfo { get => this.targetInfo; }
        [JsonIgnore] private readonly CharacterTargetInfo targetInfo = new();

        [JsonIgnore] public bool CombatExclusive
        {
            get =>
                this.Target == CharacterTarget.Opponent ||
                this.Target == CharacterTarget.AllOpponents ||
                this.Target == CharacterTarget.RandomOpponent;
        }
    }
}
