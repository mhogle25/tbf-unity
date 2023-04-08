using System.Collections.Generic;
using Newtonsoft.Json;
using BF2D.Game.Enums;
using BF2D.Utilities;

namespace BF2D
{
    public class Entity
    {
        [JsonIgnore] public string Name { get => this.name.Wash(); set => this.name = value; }
        [JsonProperty] private string name = string.Empty;
        [JsonIgnore] public string Description { get => this.description.Wash(); }
        [JsonProperty] private readonly string description = string.Empty;
        [JsonIgnore] public IEnumerable<AuraType> Auras { get => this.auras; }
        [JsonProperty] private readonly List<AuraType> auras = new();

        public bool ContainsAura(AuraType aura)
        {
            return this.auras.Contains(aura);
        }
    }
}