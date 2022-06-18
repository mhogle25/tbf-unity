using System;
using Newtonsoft.Json;
using BF2D.Game;
using System.Collections.Generic;
using BF2D.Enums;

namespace BF2D.Actions
{
    [Serializable]
    public class CharacterStatsAction
    {
        [JsonIgnore] public CharacterTarget Target { get { return this.target; } }
        [JsonProperty] private CharacterTarget target = CharacterTarget.Self;
        [JsonIgnore] public string Description { get { return this.description; } }
        [JsonProperty] private string description = "target";

        [JsonProperty] public CharacterStatsActionProperties properties = new CharacterStatsActionProperties();

        [JsonIgnore] public List<CharacterStats> Targets = null;
    }
}
