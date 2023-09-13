using System.Collections.Generic;
using Newtonsoft.Json;
using BF2D.Game.Enums;
using BF2D.Utilities;
using UnityEngine;
using BF2D.Game;

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

        public bool ContainsAura(AuraType aura) => this.auras.Contains(aura);

        public void EmbueAura(AuraType aura)
        {
            if (ContainsAura(aura))
            {
                Debug.LogWarning($"[Entity:EmbueAura] Tried to embue the aura '{aura}' onto {this.Name} but they already have that aura.");
                return;
            }

            this.auras.Add(aura);
        }

        public void RemoveAura(AuraType aura)
        {
            if (!ContainsAura(aura))
            {
                Debug.LogWarning($"[Entity:RemoveAura] Tried to remove the aura '{aura}' from {this.Name} but they don't have that aura.");
                return;
            }

            this.auras.Remove(aura);
        }
    }

    public static class EntityExtensions
    {
        public static T Setup<T>(this T entity, string id) where T : Entity
        { 
            entity.ID = id;
            return entity;
        }

        public static T Setup<T>(this T entity, string id, string name) where T : Entity
        {
            entity.ID = id;
            entity.Name = name;
            return entity;
        }

        public static T Setup<T>(this T entity, string id, string name, string description) where T : Entity
        {
            entity.ID = id;
            entity.Name = name;
            entity.Description = description;
            return entity;
        }
    }
}