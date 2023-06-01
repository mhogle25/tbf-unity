using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using BF2D.Game.Enums;

namespace BF2D.Game.Actions
{
    public class CharacterStatsActionInfo : UtilityEntityInfo
    {
        [JsonProperty] private string id = string.Empty;
        [JsonProperty] private int count = 0;

        [JsonIgnore] public override string ID { get => this.id; set => this.id = value; }
        [JsonIgnore] public override int Count => this.count;
        [JsonIgnore] public override string Name => GetEntity().Name;
        [JsonIgnore] public override Sprite Icon => GameCtx.Instance.GetIcon(GetUtility().SpriteID);
        [JsonIgnore] public override string Description => GetEntity().Description;
        [JsonIgnore] public override IEnumerable<AuraType> Auras => GetEntity().Auras;

        public CharacterStatsAction Get() => GameCtx.Instance.GetGem(this.ID);
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
