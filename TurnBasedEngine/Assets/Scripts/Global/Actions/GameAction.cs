using System.Collections.Generic;
using Newtonsoft.Json;

namespace BF2D.Game.Actions
{
    public class GameAction
    {
        [JsonIgnore] public List<string> UseMessage { get { return this.useMessage; } }
        [JsonProperty] protected readonly List<string> useMessage = new List<string>();
    }
}