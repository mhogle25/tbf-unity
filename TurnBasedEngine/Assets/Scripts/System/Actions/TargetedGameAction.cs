using System.Collections.Generic;
using Newtonsoft.Json;

namespace BF2D.Game.Actions
{
    public class TargetedGameAction : GameAction
    {
        [JsonIgnore] public List<TargetedCharacterStatsAction> TargetedGems { get { return this.targetedGems; } }
        [JsonProperty] private readonly List<TargetedCharacterStatsAction> targetedGems = new();

        [JsonIgnore] public bool CombatExclusive
        {
            get
            {
                foreach (TargetedCharacterStatsAction targetedGem in this.TargetedGems)
                    if (targetedGem.CombatExclusive)
                        return true;

                return false;
            }
        }
    }
}