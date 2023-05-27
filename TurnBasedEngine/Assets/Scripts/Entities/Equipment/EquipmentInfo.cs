using System;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;
using BF2D.Enums;

namespace BF2D.Game
{
    [Serializable]
    public class EquipmentInfo : IUtilityEntityInfo
    {
        [JsonIgnore] public string ID => this.id;
        [JsonProperty] private readonly string id = string.Empty;
        [JsonIgnore] public int Count => this.count;
        [JsonProperty] protected int count = 0;

        [JsonIgnore] public Entity GetEntity => Get();

        [JsonIgnore] public IUtilityEntity GetUtility => Get();

        [JsonIgnore] public Sprite Icon => GameInfo.Instance.GetIcon(this.GetUtility.SpriteID);

        [JsonIgnore] public string Name => Get().Name;

        [JsonIgnore] public string Description => Get().Description;

        [JsonIgnore] public IEnumerable<Enums.AuraType> Auras => Get().Auras;

        [JsonConstructor]
        public EquipmentInfo() { }

        public EquipmentInfo(string id)
        {
            this.id = id;
        }

        public Equipment Get() => GameInfo.Instance.GetEquipment(this.ID);

        public void Increment()
        {
            this.count++;
        }

        public void Decrement(CharacterStats owner)
        {
            this.count--;
            if (this.Count < 1)
                owner.Equipments.RemoveEquipment(this);
        }
    }
}
