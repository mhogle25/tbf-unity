using System;
using Newtonsoft.Json;

namespace BF2D.Game
{
    [Serializable]
    public class ItemInfo
    {
        [JsonIgnore] public string Name { get { return this.name; } set { this.name = value; } }
        [JsonProperty] protected string name = string.Empty;
        [JsonIgnore] public int Count { get { return this.count; } set { this.count = value; } }
        [JsonProperty] protected int count = 1;

        private Item item = null;

        public virtual Item Get()
        {
            this.item ??= GameInfo.Instance.GetItem(this.name);

            return this.item;
        }
    }
}
