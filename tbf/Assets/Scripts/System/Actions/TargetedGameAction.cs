using System;
using System.Threading.Tasks;
using BF2D.Game.Enums;
using Newtonsoft.Json;

namespace BF2D.Game.Actions
{
    public class TargetedGameAction : GameAction, ICombatAligned
    {
        [JsonIgnore] public TargetedCharacterActionSlot[] TargetedGemSlots => this.targetedGemSlots;
        [JsonProperty] private readonly TargetedCharacterActionSlot[] targetedGemSlots = { };

        [JsonIgnore] public bool CombatExclusive
        {
            get
            {
                foreach (TargetedCharacterActionSlot slot in this.TargetedGemSlots)
                    if (slot.CombatExclusive)
                        return true;

                return false;
            }
        }

        [JsonIgnore] public override CombatAlignment Alignment => CombatAlignmentSelector.CalculateCombatAlignedCollection(this.TargetedGemSlots);
        [JsonIgnore] public override bool IsRestoration
        {
            get
            {
                foreach (TargetedCharacterActionSlot gem in this.TargetedGemSlots)
                {
                    if (gem.IsRestoration &&
                        !gem.Chaotic &&
                        (gem.Target == CharacterTarget.Ally ||
                        gem.Target == CharacterTarget.AllAllies ||
                        gem.Target == CharacterTarget.Self ||
                        gem.Target == CharacterTarget.RandomAlly))
                        return true;
                }

                return false;
            }
        }
        
        public override string TextBreakdown(CharacterStats source)
        {
            string description = string.Empty;

            foreach (TargetedCharacterActionSlot targetedGemSlot in this.TargetedGemSlots)
                description += $"-\n{targetedGemSlot.TextBreakdown(source)}";

            return description;
        }
    }
}