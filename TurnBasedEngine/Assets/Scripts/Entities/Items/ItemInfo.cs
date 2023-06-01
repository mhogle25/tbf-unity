using System;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;

namespace BF2D.Game
{
    [Serializable]
    public class ItemInfo : UtilityEntityInfo
    {
        [JsonIgnore] public override string ID { get => this.id; set => this.id = value; }
        [JsonProperty] private string id = string.Empty;
        [JsonIgnore] public override int Count => this.count; 
        [JsonProperty] private int count = 0;

        [JsonIgnore] private Item staged = null;

        [JsonIgnore] public override Sprite Icon => GameCtx.Instance.GetIcon(GetUtility().SpriteID);

        [JsonIgnore] public override string Name => Get().Name;

        [JsonIgnore] public override string Description => Get().Description;

        [JsonIgnore] public bool Generated => Strings.System.IsGeneratedID(this.ID);

        [JsonIgnore] public override IEnumerable<Enums.AuraType> Auras => Get().Auras;

        [JsonIgnore] public bool Useable { get => Get().Useable; }

        [JsonIgnore] public bool CombatExclusive { get => Get().CombatExclusive; }

        public Item Get()
        {
            this.staged ??= GameCtx.Instance.InstantiateItem(this.id);
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

        public override int Increment()
        {
            return ++this.count;
        }

        public override int Decrement()
        {
            return --this.count;
        }

        public Item ResetStaged()
        {
            Item item = this.staged;
            this.staged = null;
            return item;
        }
    }
}
