using System;
using Newtonsoft.Json;
using UnityEngine;

namespace BF2D.Game
{
    [Serializable]
    public class EquipmentInfo : IUtilityEntityInfo
    {
        [JsonIgnore] public string ID { get => this.id; }
        [JsonProperty] private readonly string id = string.Empty;
        [JsonIgnore] public int Count { get => this.count; }
        [JsonProperty] protected int count = 0;

        [JsonProperty] private readonly object custom = null;

        [JsonIgnore] public Entity GetEntity { get => Get(); }

        [JsonIgnore] public IUtilityEntity GetUtility { get => Get(); }

        [JsonIgnore] public Sprite Icon { get => GameInfo.Instance.GetIcon(GetUtility.SpriteID); }

        [JsonIgnore] public string Name { get => Get().Name; }

        [JsonConstructor]
        public EquipmentInfo() { }

        public EquipmentInfo(string id)
        {
            this.id = id;
        }

        public EquipmentInfo(string id, Equipment customData)
        {
            this.id = id;
            this.custom = BF2D.Utilities.JSON.SerializeObject(customData);
        }

        public Equipment Get()
        {
            if (this.custom is not null)
                return BF2D.Utilities.JSON.DeserializeString<Equipment>(this.custom.ToString());
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
