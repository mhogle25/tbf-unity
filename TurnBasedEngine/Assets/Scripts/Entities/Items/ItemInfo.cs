using System;
using Newtonsoft.Json;
using UnityEngine;

namespace BF2D.Game
{
    [Serializable]
    public class ItemInfo
    {
        [JsonIgnore] public string ID { get { return this.id; } }
        [JsonProperty] protected string id = string.Empty;
        [JsonIgnore] public int Count { get { return this.count; } }
        [JsonProperty] protected int count = 1;

        private Item staged = null;

        public Item Get()
        {
            this.staged ??= GameInfo.Instance.GetItem(this.id);
            return this.staged;
        }

        public void UseItem()
        {
            if (this.count < 1)
            {
                Debug.LogWarning("[ItemInfo:UseItem] Tried to use the staged item but the item count was less than 1");
                return;
            }
            this.count--;
            ResetStagedItem();
        }

        public void ResetStagedItem()
        {
            this.staged = null;
        }
    }
}
