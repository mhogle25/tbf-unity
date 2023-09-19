using System;
using System.Collections.Generic;
using BF2D.Game.Actions;

namespace BF2D.Game.Combat.Actions
{
    public class ItemCombatActionInfo
    {
        public ItemInfo Info { get => this.info; set => this.info = value; }
        private ItemInfo info = null;

        public TargetedGameAction OnUse => this.Info.Get()?.OnUse;
        public TargetedGameAction UseOnUse() => this.Info.Use(CombatCtx.One.CurrentCharacter.Stats.Items)?.OnUse;
    }
}
