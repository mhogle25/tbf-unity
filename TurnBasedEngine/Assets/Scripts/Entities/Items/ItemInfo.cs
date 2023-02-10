using System;
using Newtonsoft.Json;
using UnityEngine;

namespace BF2D.Game
{
    [Serializable]
    public class ItemInfo : IEntityInfo
    {
        [JsonIgnore] public string ID { get { return this.id; } }
        [JsonProperty] private string id = string.Empty;
        [JsonIgnore] public int Count { get { return this.count; } }
        [JsonProperty] private int count = 0;

        [JsonIgnore] private Item staged = null;

        public ItemInfo(string id)
        {
            this.id = id;
        }

        public Item Get()
        {
            this.staged ??= GameInfo.Instance.InstantiateItem(this.id);
            return this.staged;
        }

        public void Increment()
        {
            this.count++;
        }

        public Item Use(CharacterStats owner)
        {
            Get();

            if (this.staged is null)
                return null;

            if (this.staged.Consumeable)
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
