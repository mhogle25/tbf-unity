using System;
using Newtonsoft.Json;
using UnityEngine;

namespace BF2D.Game
{
    [Serializable]
    public class ItemInfo
    {
        [JsonIgnore] public string ID { get { return this.id; } set { this.id = value; } }
        [JsonProperty] protected string id = string.Empty;
        [JsonIgnore] public int Count { get { return this.count; } }
        [JsonProperty] protected int count = 1;

        [JsonIgnore] private Item staged = null;

        public Item Get()
        {
            this.staged ??= GameInfo.Instance.InstantiateItem(this.id);
            return this.staged;
        }

        public void Increment()
        {
            this.count++;
        }

        public Item Use()
        {
            Get();
            if (this.staged.Consumeable)
                this.count--;
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
