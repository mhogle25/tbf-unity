using System;
using System.Collections.Generic;
using BF2D.Game.Actions;

namespace BF2D.Game.Combat.Actions
{
    public class ItemCombatActionInfo : ICombatActionInfo
    {
        public ItemInfo Info { get => this.info; set => this.info = value; }
        private ItemInfo info = null;

        public bool HasGems { get => this.Info.Get()?.OnUse.TargetedGemSlots.Length > 0; }

        public IEnumerable<TargetedCharacterStatsActionSlot> TargetedGemSlots => this.Info.Get()?.OnUse.TargetedGemSlots;

        public IEnumerable<TargetedCharacterStatsActionSlot> UseTargetedGemSlots() => Info.Use(CombatManager.Instance.CurrentCharacter.Stats.Items)?.OnUse.TargetedGemSlots;

        public List<string> GetOpeningMessage()
        {
            return this.info.Get()?.OnUse.Message;
        }
    }
}
