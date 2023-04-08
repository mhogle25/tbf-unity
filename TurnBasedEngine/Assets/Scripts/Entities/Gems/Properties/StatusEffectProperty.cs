using Newtonsoft.Json;
using System;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class StatusEffectProperty
    {
        [JsonIgnore] public string ID { get => this.id; }
        [JsonProperty] protected string id = string.Empty;

        [JsonIgnore] public int SuccessRate { get => this.successRate; }
        [JsonProperty] public int successRate = 100;

        public StatusEffect Get()
        {
            return GameInfo.Instance.GetStatusEffect(this.ID);
        }

        public string Run(CharacterStats source, CharacterStats target)
        {
            StatusEffect statusEffect = Get();
            string message = string.Empty;

            if (statusEffect is null)
            {
                Terminal.IO.LogError($"[CharacterStatsAction:StatusEffectProperty:Run] A status effect with id '{statusEffect}' does not exist");
            }
            else
            {
                if (Utilities.Probability.Roll(source, this.successRate))
                {
                    if (target.ApplyStatusEffect(this.ID))
                    {
                        message += source == target ?
                            $"{source.Name} {statusEffect.Description} themself with {statusEffect.Name}. {Strings.DialogTextbox.BriefPause}" :
                            $"{source.Name} {statusEffect.Description} {target.Name} with {statusEffect.Name}. {Strings.DialogTextbox.BriefPause}";
                    }
                    else
                    {
                        message += $"{target.Name} already has {statusEffect.Name}. {Strings.DialogTextbox.BriefPause}";
                    }
                }
                else
                {
                    message += $"{statusEffect.Name} failed. {Strings.DialogTextbox.BriefPause}";
                }
            }

            return message;
        }
    }
}