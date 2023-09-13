
using System;
using Newtonsoft.Json;

namespace BF2D.Game
{
    [Serializable]
    public class EncounterCharacterProperty : ICharacterInfo, ICache
    {
        [JsonIgnore]
        public CharacterStats Stats
        {
            get
            {
                if (string.IsNullOrEmpty(this.id))
                    return null;

                this.cached ??= GameCtx.One.InstantiateEnemy(this.id);

                return this.cached;
            }
        }

        [JsonIgnore] public string ID { set => this.id = value; }
        [JsonIgnore] public int Position { get => this.position; set => this.position = value; }

        [JsonIgnore] private CharacterStats cached;

        [JsonProperty] private string id = string.Empty;
        [JsonProperty] private int position = 0;

        public void Clear() => this.cached = null;
    }
}