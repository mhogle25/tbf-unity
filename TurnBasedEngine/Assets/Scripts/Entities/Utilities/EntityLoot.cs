using System;
using Newtonsoft.Json;

namespace BF2D.Game
{
    [Serializable]
    public class EntityLoot
    {
        [JsonIgnore] public string ID { get { return this.id; } }
        [JsonProperty] private string id = string.Empty;
        [JsonIgnore] public int Probability { get { return this.probability; } }
        [JsonProperty] private int probability = 100;
        [JsonIgnore] public int Count { get { return this.count; } }
        [JsonProperty] private int count = 1;
    }
}