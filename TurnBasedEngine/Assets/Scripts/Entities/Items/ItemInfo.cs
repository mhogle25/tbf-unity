using System;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;

namespace BF2D.Game
{
    [Serializable]
    public class ItemInfo : UtilityEntityInfo
    {

        [JsonIgnore] private Item staged = null;

        [JsonIgnore] public override Sprite Icon => GameCtx.Instance.GetIcon(GetUtility().SpriteID);

        [JsonIgnore] public override string Description => Get().Description;

        [JsonIgnore] public override IEnumerable<Enums.AuraType> Auras => Get().Auras;

        [JsonIgnore] public bool Useable { get => Get().Useable; }

        [JsonIgnore] public bool CombatExclusive { get => Get().CombatExclusive; }

        public Item Get()
        {
            Item item = GameCtx.Instance.InstantiateItem(this.ID);
            this.staged ??= item;
            return this.staged;
        }

        public override Entity GetEntity() => Get();

        public override IUtilityEntity GetUtility() => Get();

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
