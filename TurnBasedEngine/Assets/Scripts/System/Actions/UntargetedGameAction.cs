using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using BF2D.Game.Enums;
using UnityEngine;
using System.Linq;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class UntargetedGameAction : GameAction, ICombatAligned
    {
        [JsonProperty] private readonly CharacterStatsActionSlot[] gemSlots = { };

        [JsonIgnore] public CharacterStatsActionSlot[] GemSlots => this.gemSlots;

        [JsonIgnore] public CombatAlignment Alignment => CombatAlignmentSelector.CalculateCombatAlignedCollection(this.GemSlots);

        [JsonConstructor]
        public UntargetedGameAction() { }

        public UntargetedGameAction(CharacterStatsActionSlot[] gemSlots)
        {
            this.gemSlots = gemSlots;
        }

        public UntargetedGameAction Append(UntargetedGameAction toAppend)
        {
            if (toAppend is null)
                return this;

            return new UntargetedGameAction(this.GemSlots.Concat(toAppend.GemSlots).ToArray())
            {
                Message = this.Message
            };
        }
    }
}