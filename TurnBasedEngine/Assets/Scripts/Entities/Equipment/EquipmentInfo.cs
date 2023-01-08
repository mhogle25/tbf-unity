using System;
using Newtonsoft.Json;

namespace BF2D.Game
{
    [Serializable]
    public class EquipmentInfo
    {
        [JsonIgnore] public string Name { get { return this.name; } set { this.name = value; } }
        [JsonProperty] protected string name = string.Empty;
        [JsonIgnore] public int Count { get { return this.count; } set { this.count = value; } }
        [JsonProperty] protected int count = 1;
        [JsonIgnore] public bool Equipped { get { return this.equipped; } }
        [JsonProperty] public bool equipped = false;

        public Equipment Get()
        {
            return GameInfo.Instance.GetEquipment(this.name);
        }
    }
}
