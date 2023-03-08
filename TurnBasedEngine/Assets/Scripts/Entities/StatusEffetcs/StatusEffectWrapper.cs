using System;
using Newtonsoft.Json;

namespace BF2D.Game
{
    [Serializable]
    public abstract class StatusEffectWrapper
    {
        [JsonIgnore] public string ID { get { return this.id; } }
        [JsonProperty] protected string id = string.Empty;

        public StatusEffect Get()
        {
            return GameInfo.Instance.GetStatusEffect(this.id);
        }
    }
}