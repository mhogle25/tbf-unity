using System.Collections.Generic;
using Newtonsoft.Json;

namespace BF2D.Game.Actions
{
    public class GameAction
    {
        [JsonIgnore] public List<CharacterStatsAction> StatsActions { get { return this.statsAction; } }
        [JsonProperty] private readonly List<CharacterStatsAction> statsAction = new();
    }
}