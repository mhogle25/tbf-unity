using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using BF2D.Game.Enums;

namespace BF2D.Game
{
    public abstract class UtilityEntityInfo : IEntityInfo
    {
        [JsonProperty] private string id = string.Empty;
        [JsonProperty] private int count = 0;

        [JsonIgnore] public string ID { get => this.id; set => this.id = value; }
        [JsonIgnore] public int Count => this.count;

        [JsonIgnore] public abstract string Name { get; }

        [JsonIgnore] public abstract string Description { get; }

        [JsonIgnore] public abstract IEnumerable<Enums.AuraType> Auras { get; }

        [JsonIgnore] public bool Generated => Strings.System.IsGeneratedID(this.ID);

        public abstract IUtilityEntity GetUtility();

        public abstract bool ContainsAura(AuraType aura);

        [JsonIgnore] public abstract Sprite Icon { get; }

        public int Increment()
        {
            return ++this.count;
        }

        public int Decrement()
        {
            return --this.count;
        }
    }
}