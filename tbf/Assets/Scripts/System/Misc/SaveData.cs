using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BF2D.Game
{
    [Serializable]
    public class SaveData
    {
        [JsonIgnore] public string ID { get => this.id; }
        [JsonIgnore] private string id = string.Empty;

        [JsonIgnore] public Party Party => this.party;
        [JsonProperty] private readonly Party party = null;

        public SaveData Setup(string id)
        {
            this.id = id;
            return this;
        }
    }
}
