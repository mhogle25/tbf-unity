using Newtonsoft.Json;
using System;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class StatusEffectProperty : StatusEffectWrapper
    {
        [JsonIgnore] public int SuccessRate { get { return this.successRate; } }
        [JsonProperty] public int successRate = 100;
    }
}