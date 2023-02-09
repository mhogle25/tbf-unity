using System.Collections.Generic;
using Newtonsoft.Json;

namespace BF2D.Game.Actions
{
    public class TargetedGameAction : GameAction
    {
        [JsonIgnore] public List<TargetedCharacterStatsAction> TargetedGems { get { return this.targetedGems; } }
        [JsonProperty] private readonly List<TargetedCharacterStatsAction> targetedGems = new();
    }
}