using Newtonsoft.Json;

namespace BF2D.Game
{
    public class Entity
    {
        [JsonIgnore] public string Name { get { return this.name; } }
        [JsonProperty] protected string name = string.Empty;
        [JsonIgnore] public string Description { get { return this.description; } }
        [JsonProperty] protected readonly string description = string.Empty;
    }
}