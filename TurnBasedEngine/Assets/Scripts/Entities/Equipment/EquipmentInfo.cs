using System;
using Newtonsoft.Json;

namespace BF2D.Game
{
    [Serializable]
    public class EquipmentInfo
    {
        [JsonIgnore] public string ID { get { return this.id; } set { this.id = value; } }
        [JsonProperty] protected string id = string.Empty;
        [JsonIgnore] public int Count { get { return this.count; } }
        [JsonProperty] protected int count = 1;

        public Equipment Get()
        {
            return GameInfo.Instance.GetEquipment(this.ID);
        }
    }
}
