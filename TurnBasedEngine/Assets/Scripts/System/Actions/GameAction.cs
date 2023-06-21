using System.Collections.Generic;
using Newtonsoft.Json;

namespace BF2D.Game.Actions
{
    public class GameAction
    {
        [JsonIgnore] public List<string> Message { get => this.message; set => this.message = value; }
        [JsonProperty] protected List<string> message = new();
    }
}