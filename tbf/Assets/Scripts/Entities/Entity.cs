using System.Collections.Generic;
using Newtonsoft.Json;
using BF2D.Game.Enums;
using BF2D.Utilities;
using UnityEngine;

namespace BF2D
{
    [System.Serializable]
    public abstract class Entity
    {
        [JsonIgnore] public abstract string ID { get; set; }
        [JsonIgnore] public string Name { get => this.name.Wash(); set => this.name = value; }
        [JsonIgnore] public string Description { get => this.description.Wash(); set => this.description = value; }
        [JsonIgnore] public IEnumerable<AuraType> Auras => this.auras;
        [JsonIgnore] public bool Chaotic => ContainsAura(AuraType.Chaos);

        [JsonProperty] private string name = string.Empty;
        [JsonProperty] private string description = string.Empty;
        [JsonProperty] private readonly HashSet<AuraType> auras = new();

        public T Setup<T>(string id) where T : Entity
        {
            this.ID = id;
            return this as T;
        }

        public T Setup<T>(string id, string name) where T : Entity
        {
            this.ID = id;
            this.name = name;
            return this as T;
        }

        public T Setup<T>(string id, string name, string description) where T : Entity
        {
            this.ID = id;
            this.name = name;
            this.description = description;
            return this as T;
        }

        public bool ContainsAura(AuraType aura) => this.auras.Contains(aura);

        public void EmbueAura(AuraType aura)
        {
            if (ContainsAura(aura))
            {
                Debug.LogWarning($"[Entity:EmbueAura] Tried to embue the aura '{aura}' onto {this.Name} but {this.Name} already has that aura.");
                return;
            }

            this.auras.Add(aura);
        }

        public void RemoveAura(AuraType aura)
        {
            if (!ContainsAura(aura))
            {
                Debug.LogWarning($"[Entity:RemoveAura] Tried to remove the aura '{aura}' from {this.Name} but {this.Name} doesn't have that aura.");
                return;
            }

            this.auras.Remove(aura);
        }
    }
}