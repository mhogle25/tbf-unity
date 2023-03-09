using System;
using Newtonsoft.Json;
using UnityEngine;

namespace BF2D.Game
{
    [Serializable]
    public class EquipmentInfo : IUtilityEntityInfo
    {
        [JsonIgnore] public string ID { get { return this.id; } }
        [JsonProperty] private readonly string id = string.Empty;
        [JsonIgnore] public int Count { get { return this.count; } }
        [JsonProperty] protected int count = 0;

        [JsonIgnore] public Entity GetEntity { get { return Get(); } }

        [JsonIgnore] public IUtilityEntity GetUtility { get { return Get(); } }

        [JsonIgnore] public Sprite Icon { get { return GameInfo.Instance.GetIcon(GetUtility.SpriteID); } }

        [JsonIgnore] public string Name { get { return Get().Name; } }

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
