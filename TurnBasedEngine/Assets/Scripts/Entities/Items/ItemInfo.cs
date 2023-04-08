using System;
using Newtonsoft.Json;
using UnityEngine;

namespace BF2D.Game
{
    [Serializable]
    public class ItemInfo : IUtilityEntityInfo
    {
        [JsonIgnore] public string ID { get => this.id; }
        [JsonProperty] private readonly string id = string.Empty;
        [JsonIgnore] public int Count { get => this.count; }
        [JsonProperty] private int count = 0;

        [JsonProperty] private readonly object custom = null;

        [JsonIgnore] private Item staged = null;

        [JsonIgnore] public Entity GetEntity { get => Get(); }

        [JsonIgnore] public IUtilityEntity GetUtility { get => Get(); }

        [JsonIgnore] public Sprite Icon { get => GameInfo.Instance.GetIcon(GetUtility.SpriteID); }

        [JsonIgnore] public string Name { get => Get().Name; }

        [JsonIgnore] public bool Useable { get => Get().Useable; }

        [JsonIgnore] public bool CombatExclusive { get => Get().CombatExclusive; }

        [JsonConstructor]
        public ItemInfo() { }

        public ItemInfo(string id)
        {
            this.id = id;
        }

        public ItemInfo(string id, Item customData)
        {
            this.id = id;
            this.custom = Utilities.JSON.SerializeObject(customData);
        }

        public Item Get()
        {
            if (this.custom is not null)
                this.staged ??= Utilities.JSON.DeserializeString<Item>(this.custom.ToString());
            else 
                this.staged ??= GameInfo.Instance.InstantiateItem(this.id);

            return this.staged;
        }

        public void Increment()
        {
            this.count++;
        }

        public Item Use(IItemHolder owner)
        {
            Get();

            if (this.staged is null)
                return null;

            if (this.staged.Consumable)
            {
                this.count--;
                if (this.count < 1)
                    owner.RemoveItem(this);
            }

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
