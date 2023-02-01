using System;
using Newtonsoft.Json;

namespace BF2D.Game
{
    [Serializable]
    public class EquipmentInfo
    {
        [JsonIgnore] public string ID { get { return this.id; } }
        [JsonProperty] protected string id = string.Empty;
        [JsonIgnore] public int Count { get { return this.count; } }
        [JsonProperty] protected int count = 0;

        public EquipmentInfo(string id)
        {
            this.id = id;
        }

        public Equipment Get()
        {
            return GameInfo.Instance.GetEquipment(this.ID);
        }

        public void Increment()
        {
            this.count++;
        }

        public void Decrement(CharacterStats owner)
        {
            this.count--;
            if (this.Count < 1)
                owner.RemoveEquipment(this);
        }
    }
}
