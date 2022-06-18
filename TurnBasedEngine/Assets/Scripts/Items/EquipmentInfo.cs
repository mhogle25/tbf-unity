using System;
using Newtonsoft.Json;

namespace BF2D.Game
{
    [Serializable]
    public class EquipmentInfo : ItemInfo
    {
        [JsonIgnore] public bool Equipped { get { return this.equipped; } }
        [JsonProperty] public bool equipped = false;

        public override Item Get()
        {
            return GameInfo.Instance.GetEquipment(this.name);
        }
    }
}
