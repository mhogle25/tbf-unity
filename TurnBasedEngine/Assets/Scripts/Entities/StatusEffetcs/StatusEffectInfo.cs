

using Newtonsoft.Json;

namespace BF2D.Game
{
    public class StatusEffectInfo
    {
        [JsonIgnore] public string ID { get { return this.id; } }
        [JsonProperty] protected string id = string.Empty;
        [JsonIgnore] public int RemainingDuration { get { return this.remainingDuration; } }
        [JsonProperty] private int remainingDuration = -1;

        public StatusEffectInfo(string id)
        {
            this.id = id;
            this.remainingDuration = GameInfo.Instance.GetStatusEffect(id).Duration;
        }

        public StatusEffect Get()
        {
            return GameInfo.Instance.GetStatusEffect(this.ID);
        }

        public void Use(CharacterStats owner)
        {
            if (this.remainingDuration < 0)
                return;

            this.remainingDuration--;

            if (this.remainingDuration == 0)
            {
                owner.RemoveStatusEffect(this);
                return;
            }
        }
    }
}