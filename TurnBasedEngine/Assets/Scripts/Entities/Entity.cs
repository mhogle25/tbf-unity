using System.Collections.Generic;
using Newtonsoft.Json;
using BF2D.Game.Enums;
using BF2D.Utilities;

namespace BF2D
{
    public abstract class Entity
    {
        [JsonIgnore] public abstract string ID { get; set; }
        [JsonIgnore] public string Name { get => this.name.Wash(); set => this.name = value; }
        [JsonIgnore] public string Description { get => this.description.Wash(); }
        [JsonIgnore] public IEnumerable<AuraType> Auras { get => this.auras; }

        [JsonProperty] private string name = string.Empty;
        [JsonProperty] private readonly string description = string.Empty;
        [JsonProperty] private readonly List<AuraType> auras = new();

        public T Setup<T>(string id) where T : Entity
        {
            this.ID = id;
            return (T) this;
        }

        public T Setup<T>(string id, string name) where T : Entity
        {
            this.ID = id;
            this.name = name;
            return (T) this;
        }

        public bool ContainsAura(AuraType aura)
        {
            return this.auras.Contains(aura);
        }
    }
}