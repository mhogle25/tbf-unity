using System;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;

namespace BF2D.Game
{
    [Serializable]
    public class ItemInfo : IUtilityEntityInfo
    {
        [JsonIgnore] public string ID => this.id; 
        [JsonProperty] private readonly string id = string.Empty;
        [JsonIgnore] public int Count => this.count; 
        [JsonProperty] private int count = 0;

        [JsonIgnore] private Item staged = null;

        [JsonIgnore] public Sprite Icon => GameCtx.Instance.GetIcon(GetUtility().SpriteID);

        [JsonIgnore] public string Name => Get().Name;

        [JsonIgnore] public string Description => Get().Description;

        [JsonIgnore] public bool Generated => Strings.System.IsGeneratedID(this.ID);

        [JsonIgnore] public IEnumerable<Enums.AuraType> Auras => Get().Auras;

        [JsonIgnore] public bool Useable { get => Get().Useable; }

        [JsonIgnore] public bool CombatExclusive { get => Get().CombatExclusive; }

        [JsonConstructor]
        public ItemInfo() { }

        public ItemInfo(string id)
        {
            this.id = id;
        }

        public Item Get()
        {
            this.staged ??= GameCtx.Instance.InstantiateItem(this.id);
            return this.staged;
        }

        public Entity GetEntity() => Get();

        public IUtilityEntity GetUtility() => Get();

        public Item Use(IItemHolder owner)
        {
            Get();

            if (this.staged is null)
                return null;

            if (this.staged.Consumable)
                owner.RemoveItem(this);

            return ResetStaged();
        }

        public int Increment()
        {
            return ++this.count;
        }

        public int Decrement()
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
