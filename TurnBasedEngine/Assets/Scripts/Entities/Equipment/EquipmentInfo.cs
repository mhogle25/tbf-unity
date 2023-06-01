using System;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;
using BF2D.Enums;

namespace BF2D.Game
{
    [Serializable]
    public class EquipmentInfo : UtilityEntityInfo
    {
        [JsonIgnore] public override string ID { get => this.id; set => this.id = value; }
        [JsonProperty] private string id = string.Empty;
        [JsonIgnore] public override int Count => this.count;
        [JsonProperty] protected int count = 0;

        [JsonIgnore] public override Sprite Icon => GameCtx.Instance.GetIcon(this.GetUtility().SpriteID);

        [JsonIgnore] public override string Name => Get().Name;

        [JsonIgnore] public override string Description => Get().Description;

        [JsonIgnore] public override IEnumerable<Enums.AuraType> Auras => Get().Auras;

        [JsonIgnore] public bool Generated => Strings.System.IsGeneratedID(this.ID);

        public Equipment Get() => GameCtx.Instance.GetEquipment(this.ID);

        public override Entity GetEntity() => Get();

        public override IUtilityEntity GetUtility() => Get();

        public override int Increment()
        {
            return ++this.count;
        }

        public override int Decrement()
        {
            return --this.count;
        }
    }
}
