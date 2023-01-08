using System.Collections.Generic;
using Newtonsoft.Json;

namespace BF2D.Game.Actions
{
    public class TargetedGameAction : GameAction
    {
        [JsonIgnore] public List<CharacterStatsAction> StatsActions { get { return this.statsActions; } }
        [JsonProperty] private readonly List<CharacterStatsAction> statsActions = new();
    }
}