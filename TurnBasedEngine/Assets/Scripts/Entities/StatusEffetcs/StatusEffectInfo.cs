using Newtonsoft.Json;

namespace BF2D.Game
{
    public class StatusEffectInfo : StatusEffectWrapper, IEntityInfo
    {
        [JsonIgnore] public int RemainingDuration { get { return this.remainingDuration; } }
        [JsonProperty] private int remainingDuration = -1;

        [JsonIgnore] public Entity GetEntity { get { return Get(); } }

        public StatusEffectInfo(string id)
        {
            this.id = id;
            this.remainingDuration = Get().Duration;
        }

        public void Use()
        {
            if (this.remainingDuration < 0)
                return;

            this.remainingDuration--;
        }
    }
}