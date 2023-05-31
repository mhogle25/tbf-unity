using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using BF2D.Game.Enums;

namespace BF2D.Game.Actions
{
    public class CharacterStatsActionInfo : IUtilityEntityInfo
    {
        [JsonProperty] private readonly string id = string.Empty;
        [JsonProperty] private int count = 0;

        [JsonIgnore] public string ID => this.id;
        [JsonIgnore] public int Count => this.count;
        [JsonIgnore] public string Name => GetEntity().Name;
        [JsonIgnore] public Sprite Icon => GameCtx.Instance.GetIcon(GetUtility().SpriteID);
        [JsonIgnore] public string Description => GetEntity().Description;
        [JsonIgnore] public IEnumerable<AuraType> Auras => GetEntity().Auras;

        public CharacterStatsAction Get() => GameCtx.Instance.GetGem(this.ID);
        public Entity GetEntity() => Get();
        public IUtilityEntity GetUtility() => Get();

        [JsonConstructor]
        public CharacterStatsActionInfo() { }
        public CharacterStatsActionInfo(string id) => this.id = id;

        public void Increment()
        {
            this.count++;
        }

        public void Decrement(ICharacterStatsActionHolder owner)
        {
            if (--this.count < 1)
                owner.RemoveGem(this);
        }
    }
}
