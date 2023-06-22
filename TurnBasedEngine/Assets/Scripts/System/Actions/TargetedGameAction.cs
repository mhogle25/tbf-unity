using BF2D.Game.Enums;
using Newtonsoft.Json;

namespace BF2D.Game.Actions
{
    public class TargetedGameAction : GameAction, ICombatAligned
    {
        [JsonIgnore] public TargetedCharacterStatsAction[] TargetedGemSlots => this.targetedGemSlots;
        [JsonProperty] private readonly TargetedCharacterStatsAction[] targetedGemSlots = { };

        [JsonIgnore] public bool CombatExclusive
        {
            get
            {
                foreach (TargetedCharacterStatsAction targetedGem in this.TargetedGemSlots)
                    if (targetedGem.CombatExclusive)
                        return true;

                return false;
            }
        }

        [JsonIgnore] public CombatAlignment Alignment => CombatAlignmentSelector.CalculateCombatAlignedCollection(this.TargetedGemSlots);
    }
}