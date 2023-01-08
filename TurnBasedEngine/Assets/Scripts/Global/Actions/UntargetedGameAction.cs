using System.Collections.Generic;
using Newtonsoft.Json;

namespace BF2D.Game.Actions
{
    public class UntargetedGameAction
    {
        [JsonIgnore] public List<CharacterStatsActionProperties> StatsActions { get { return this.statsActions; } }
        [JsonProperty] private readonly List<CharacterStatsActionProperties> statsActions = new();
    }
}