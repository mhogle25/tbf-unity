using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using BF2D.Game.Enums;
using System.Linq;
using System.Threading.Tasks;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class UntargetedGameAction : GameAction, ICombatAligned
    {
        [JsonProperty] private readonly CharacterActionSlot[] gemSlots = { };

        [JsonIgnore] public CharacterActionSlot[] GemSlots => this.gemSlots;

        [JsonIgnore] public override CombatAlignment Alignment => CombatAlignmentSelector.CalculateCombatAlignedCollection(this.GemSlots);
        [JsonIgnore] public override bool IsRestoration
        {
            get
            {
                foreach (CharacterActionSlot gem in this.GemSlots)
                    if (gem.IsRestoration)
                        return true;

                return false;
            }
        }

        [JsonConstructor]
        public UntargetedGameAction() { }

        public UntargetedGameAction(CharacterActionSlot[] gemSlots, List<string> message, bool messageRandom)
        {
            this.gemSlots = gemSlots;
            this.message = message;
            this.messageRandom = messageRandom;
        }

        public UntargetedGameAction Append(UntargetedGameAction toAppend) => toAppend is null
            ? this
            : new UntargetedGameAction(this.GemSlots.Concat(toAppend.GemSlots).ToArray(), this.message, this.messageRandom);

        public override string TextBreakdown(CharacterStats source)
        {
            string description = string.Empty;

            foreach (CharacterActionSlot targetedGemSlot in this.GemSlots)
                description += $"-\n{targetedGemSlot.TextBreakdown(source)}";

            return description;
        }
    }
} 