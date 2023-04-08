using System;
using System.Collections.Generic;
using BF2D.Game.Actions;

namespace BF2D.Game.Combat.Actions
{
    public class ItemCombatActionInfo : ICombatActionInfo
    {
        public ItemInfo Info { get => this.info; set => this.info = value; }
        private ItemInfo info = null;

        public bool HasGems { get => this.Info.Get()?.OnUse.TargetedGems.Length > 0; }

        public IEnumerable<TargetedCharacterStatsAction> Gems => this.Info.Get()?.OnUse.TargetedGems;

        public IEnumerable<TargetedCharacterStatsAction> UseGems() => Info.Use(CombatManager.Instance.CurrentCharacter.Stats.Items)?.OnUse.TargetedGems;

        public List<string> GetOpeningMessage()
        {
            return this.info.Get()?.OnUse.Message;
        }
    }
}
