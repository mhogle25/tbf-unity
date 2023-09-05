using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using BF2D.Game.Enums;

namespace BF2D.Game
{
    public abstract class UtilityEntityInfo : IEntityInfo
    {
        [JsonProperty] private int count = 0;
        [JsonProperty] private string id = string.Empty;

        [JsonIgnore] public int Count => this.count;
        [JsonIgnore] public string ID { get => this.id; set => this.id = value; }
        [JsonIgnore] public bool Generated => Strings.System.IsGeneratedID(this.ID);

        [JsonIgnore] public abstract string Name { get; }
        [JsonIgnore] public abstract string Description { get; }
        [JsonIgnore] public abstract IEnumerable<AuraType> Auras { get; }
        [JsonIgnore] public CombatAlignment Alignment => GetUtility().Alignment;
        [JsonIgnore] public bool IsRestoration => GetUtility().IsRestoration;

        protected abstract IUtilityEntity GetUtility();

        public abstract bool ContainsAura(AuraType aura);

        public Sprite GetIcon() => GetUtility().GetIcon();

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