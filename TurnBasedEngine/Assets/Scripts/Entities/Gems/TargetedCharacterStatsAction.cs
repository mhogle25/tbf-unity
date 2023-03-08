using BF2D.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class TargetedCharacterStatsAction
    {
        [JsonIgnore] public CharacterTarget Target { get { return this.target; } }
        [JsonProperty] private readonly CharacterTarget target = CharacterTarget.Self;
        [JsonIgnore] public string Description { get { return this.description; } }
        [JsonProperty] private readonly string description = "target";
        [JsonIgnore] public CharacterStatsAction Gem { get { return GameInfo.Instance.GetCharacterStatsAction(this.gemID); } }
        [JsonProperty] private readonly string gemID = string.Empty;

        [JsonIgnore] public CharacterTargetInfo TargetInfo { get { return this.targetInfo; } }
        [JsonIgnore] private readonly CharacterTargetInfo targetInfo = new();
    }
}
