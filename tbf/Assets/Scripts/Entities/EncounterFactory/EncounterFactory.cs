using System;
using Newtonsoft.Json;

namespace BF2D.Game
{
    [Serializable]
    public class EncounterFactory : Entity
    {
        [JsonIgnore] public override string ID { get => this.id; set => this.id = value; }
        [JsonIgnore] public string id = string.Empty;

        [JsonProperty] private readonly Encounter encounter = null;

        public Encounter InstantiateEncounter()
        {
            return this.encounter;
        }
    }
}