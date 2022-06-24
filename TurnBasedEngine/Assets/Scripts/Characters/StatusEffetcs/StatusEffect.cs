using Newtonsoft.Json;
using BF2D.Enums;
using BF2D.Game.Actions;

namespace BF2D.Game
{
    public class StatusEffect
    {
        [JsonIgnore] public string Name { get { return this.name; } }
        [JsonProperty] protected string name = string.Empty;
        [JsonIgnore] public string Icon { get { return this.icon; } }
        [JsonProperty] protected string icon = string.Empty;
        [JsonIgnore] public string Description { get { return this.description; } }
        [JsonProperty] protected string description = string.Empty;
        [JsonIgnore] public int Duration { get { return this.duration; } }
        [JsonProperty] private int duration = -1;
        [JsonIgnore] public EffectType Effect { get { return this.effect; } }
        [JsonProperty] private EffectType effect = EffectType.Generic;
        [JsonIgnore] public int SpeedModifier { get { return this.speedModifier; } }
        [JsonProperty] private int speedModifier = 0;
        [JsonIgnore] public int AttackModifier { get { return this.attackModifier; } }
        [JsonProperty] private int attackModifier = 0;
        [JsonIgnore] public int DefenseModifier { get { return this.defenseModifier; } }
        [JsonProperty] private int defenseModifier = 0;
        [JsonIgnore] public int FocusModifier { get { return this.focusModifier; } }
        [JsonProperty] private int focusModifier = 0;
        [JsonIgnore] public int LuckModifier { get { return this.luckModifier; } }
        [JsonProperty] private int luckModifier = 0;
        [JsonIgnore] public GameAction Repeated { get { return this.repeated; } }
        [JsonProperty] private GameAction repeated = null;
    }
}
