using BF2D.Game.Enums;
using Newtonsoft.Json;

namespace BF2D.Game.Actions
{
    public class TargetedGameAction : GameAction, ICombatAligned
    {
        [JsonIgnore] public TargetedCharacterStatsActionSlot[] TargetedGemSlots => this.targetedGemSlots;
        [JsonProperty] private readonly TargetedCharacterStatsActionSlot[] targetedGemSlots = { };

        [JsonIgnore] public bool CombatExclusive
        {
            get
            {
                foreach (TargetedCharacterStatsActionSlot slot in this.TargetedGemSlots)
                    if (slot.CombatExclusive)
                        return true;

                return false;
            }
        }

        [JsonIgnore] public CombatAlignment Alignment => CombatAlignmentSelector.CalculateCombatAlignedCollection(this.TargetedGemSlots);
    }
}