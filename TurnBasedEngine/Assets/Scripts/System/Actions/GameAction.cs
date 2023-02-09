using System.Collections.Generic;
using Newtonsoft.Json;

namespace BF2D.Game.Actions
{
    public class GameAction
    {
        [JsonIgnore] public List<string> Message { get { return this.message; } }
        [JsonProperty] protected readonly List<string> message = new();
    }
}