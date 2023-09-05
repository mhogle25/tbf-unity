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

        public IEnumerable<TargetedCharacterActionSlot> TargetedGemSlots => this.Info.Get()?.OnUse.TargetedGemSlots;

        public IEnumerable<TargetedCharacterActionSlot> UseTargetedGems() => this.Info.Use(CombatCtx.One.CurrentCharacter.Stats.Items)?.OnUse.TargetedGemSlots;

        public List<string> GetOpeningMessage() => this.info.Get()?.OnUse.Message;
    }
}
