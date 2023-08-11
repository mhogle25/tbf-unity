using System;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;
using BF2D.Game.Enums;

namespace BF2D.Game
{
    [Serializable]
    public class ItemInfo : UtilityEntityInfo
    {

        [JsonIgnore] private Item staged = null;

        [JsonIgnore] public override Sprite Icon => GameCtx.One.GetIcon(GetUtility().SpriteID);

        [JsonIgnore] public override string Name => Get().Name;

        [JsonIgnore] public override string Description => Get().Description;

        [JsonIgnore] public override IEnumerable<Enums.AuraType> Auras => Get().Auras;

        [JsonIgnore] public bool Useable { get => Get().Useable; }

        [JsonIgnore] public bool CombatExclusive { get => Get().CombatExclusive; }

        public Item Get()
        {
            Item item = GameCtx.One.InstantiateItem(this.ID);
            this.staged ??= item;
            return this.staged;
        }

        public override IUtilityEntity GetUtility() => Get();

        public override bool ContainsAura(AuraType aura) => Get().ContainsAura(aura);

        public Item Use(IItemHolder owner)
        {
            Get();

            if (this.staged is null)
                return null;

            if (this.staged.Consumable)
                owner.Destroy(this);

            return ResetStaged();
        }

        public Item ResetStaged()
        {
            Item item = this.staged;
            this.staged = null;
            return item;
        }
    }
}
