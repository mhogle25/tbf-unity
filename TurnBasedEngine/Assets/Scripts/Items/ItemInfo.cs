using System;
using Newtonsoft.Json;

namespace BF2D.Game
{
    [Serializable]
    public class ItemInfo
    {
        [JsonIgnore] public string Name { get { return this.name; } }
        [JsonProperty] protected string name = string.Empty;
        [JsonIgnore] public int Count { get { return this.count; } set { this.Count = value; } }
        [JsonProperty] protected int count = 1;

        public virtual Item Get()
        {
            return GameInfo.Instance.GetItem(this.name);
        }
    }
}
