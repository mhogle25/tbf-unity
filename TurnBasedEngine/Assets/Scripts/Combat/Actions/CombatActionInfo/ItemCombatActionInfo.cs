using System;
using System.Collections.Generic;
using BF2D.Game;

namespace BF2D.Combat.Actions
{
    public class ItemCombatActionInfo : ICombatActionInfo
    {
        public ItemInfo Info { get { return this.info; } set { this.info = value; } }
        private ItemInfo info = null;
        public Queue<TargetedStatsAction> TargetedActions { get { return this.targetedActions; } }
        private readonly Queue<TargetedStatsAction> targetedActions = new();
        public List<string> GetOpeningMessage()
        {
            return this.info.Get().OnUse.UseMessage;
        }
    }
}