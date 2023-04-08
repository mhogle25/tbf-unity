using Newtonsoft.Json;

namespace BF2D.Game
{
    public class StatusEffectInfo : IEntityInfo
    {

        [JsonIgnore] public string ID { get { return this.id; } }
        [JsonProperty] protected string id = string.Empty;

        [JsonIgnore] public int RemainingDuration { get { return this.remainingDuration; } }
        [JsonProperty] private int remainingDuration = -1;

        [JsonIgnore] public Entity GetEntity { get { return Get(); } }

        public StatusEffectInfo(string id)
        {
            this.id = id;
            this.remainingDuration = Get().Duration;
        }

        public StatusEffect Get()
        {
            return GameInfo.Instance.GetStatusEffect(this.ID);
        }

        public StatusEffect Use()
        {
            if (this.remainingDuration < 1)
                return Get();

            this.remainingDuration--;
            return Get();
        }
    }
}