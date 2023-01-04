using System.Collections.Generic;
using BF2D.Game;

namespace BF2D.Combat.Actions
{
    public class ItemCombatActionInfo
    {
        public ItemInfo Info { get { return this.info; } set { this.info = value; } }
        private ItemInfo info = null;
        public Queue<TargetedStatsAction> TargetedActions { get { return this.targetedActions; } }
        private readonly Queue<TargetedStatsAction> targetedActions = new();
    }
}
