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
        [JsonProperty] private readonly UntargetedCharacterStatsAction[] untargetedGems = { };

        [JsonIgnore] public UntargetedCharacterStatsAction[] Gems => this.untargetedGems;

        [JsonIgnore] public CombatAlignment Alignment => CombatAlignmentSelector.CalculateCombatAlignedCollection(this.Gems);

        [JsonConstructor]
        public UntargetedGameAction() { }

        public UntargetedGameAction(UntargetedCharacterStatsAction[] gemIDs)
        {
            this.untargetedGems = gemIDs;
        }

        public UntargetedGameAction Append(UntargetedGameAction toAppend)
        {
            if (toAppend is null)
                return this;

            return new UntargetedGameAction(this.Gems.Concat(toAppend.Gems).ToArray())
            {
                Message = this.Message
            };
        }
    }
}