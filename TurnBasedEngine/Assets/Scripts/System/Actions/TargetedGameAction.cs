using BF2D.Game.Enums;
using Newtonsoft.Json;

namespace BF2D.Game.Actions
{
    public class TargetedGameAction : GameAction, ICombatAligned
    {
        [JsonIgnore] public TargetedCharacterStatsAction[] TargetedGems => this.targetedGems;
        [JsonProperty] private readonly TargetedCharacterStatsAction[] targetedGems = { };

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

        [JsonIgnore] public CombatAlignment Alignment => CombatAlignmentSelector.CalculateCombatAlignedCollection(this.TargetedGems);


    }
}