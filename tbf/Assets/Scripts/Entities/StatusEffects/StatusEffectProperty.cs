using BF2D.Game.Enums;
using Newtonsoft.Json;
using System;
using UnityEngine;
using BF2D.Utilities;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class StatusEffectProperty
    {
        [JsonIgnore] public string ID { get => this.id; }
        [JsonProperty] private readonly string id = string.Empty;

        [JsonIgnore] public int SuccessRate { get => this.successRate; }
        [JsonProperty] private readonly int successRate = 100;

        public StatusEffect Get()
        {
            return GameCtx.One.GetStatusEffect(this.ID);
        }

        public string Run(CharacterStats source, CharacterStats target)
        {
            StatusEffect statusEffect = Get();
            string message = string.Empty;

            if (statusEffect is null)
            {
                Debug.LogError($"[CharacterStatsAction:StatusEffectProperty:Run] A status effect with id '{statusEffect}' does not exist");
            }
            else
            {
                if (Probability.Roll(this.successRate, source.Luck))
                {
                    if (target.ApplyStatusEffect(this.ID))
                    {
                        message += source == target ?
                            $"{source.Name} {statusEffect.Description} themself with {statusEffect.Name}. {Strings.DialogTextbox.PAUSE_BREIF}" :
                            $"{source.Name} {statusEffect.Description} {target.Name} with {statusEffect.Name}. {Strings.DialogTextbox.PAUSE_BREIF}";
                    }
                    else
                    {
                        message += $"{target.Name} already has {statusEffect.Name}. {Strings.DialogTextbox.PAUSE_BREIF}";
                    }
                }
                else
                {
                    message += $"{statusEffect.Name} failed. {Strings.DialogTextbox.PAUSE_BREIF}";
                }
            }

            return message;
        }

        public string TextBreakdown(CharacterStats source)
        {
            StatusEffect effect = Get();
            string text = effect.TextBreakdown(source);
            if (this.SuccessRate < 100)
                text += $" {this.SuccessRate}%{source?.GetModifierText(CharacterStatsProperty.Luck)} chance".Colorize(Colors.Cyan);
            text += '\n';
            return text;
        }

        public string TextBreakdownSimple(CharacterStats source)
        {
            StatusEffect effect = Get();
            string text = $"{effect.Name}";
            if (this.SuccessRate < 100)
                text += $" {this.SuccessRate}%{source?.GetModifierText(CharacterStatsProperty.Luck)} chance".Colorize(Colors.Cyan);
            if (!effect.Singleton)
                text += ", stackable";
            text += '\n';
            return text;
        }
    }
}