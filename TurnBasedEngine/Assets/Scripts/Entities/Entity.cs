using System.Collections.Generic;
using Newtonsoft.Json;
using BF2D.Game.Enums;
using BF2D.Utilities;

namespace BF2D
{
    public class Entity
    {
        [JsonIgnore] public string Name { get { return this.name.Wash(); } }
        [JsonProperty] protected string name = string.Empty;
        [JsonIgnore] public string Description { get { return this.description.Wash(); } }
        [JsonProperty] protected readonly string description = string.Empty;
        [JsonIgnore] public IEnumerable<AuraType> Auras { get { return this.auras; } }
        [JsonProperty] protected readonly List<AuraType> auras = new();

        public bool ContainsAura(AuraType aura)
        {
            return this.auras.Contains(aura);
        }
    }
}