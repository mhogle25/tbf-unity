using System.Collections.Generic;
using Newtonsoft.Json;

namespace BF2D.Game.Actions
{
    public class UntargetedGameAction : GameAction
    {
        [JsonIgnore] public List<CharacterStatsActionProperties> StatsActionProperties { get { return this.statsActionProperties; } }
        [JsonProperty] private readonly List<CharacterStatsActionProperties> statsActionProperties = new();
    }
}