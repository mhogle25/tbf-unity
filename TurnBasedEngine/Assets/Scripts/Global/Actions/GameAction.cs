using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class GameAction
    {
        [JsonIgnore] public List<CharacterStatsAction> StatsActions { get { return this.statsActions; } }
        [JsonProperty] private List<CharacterStatsAction> statsActions = null;
    }
}
