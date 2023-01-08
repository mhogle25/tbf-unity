using Newtonsoft.Json;
using BF2D.Enums;
using BF2D.Game.Actions;
using System;

namespace BF2D.Game
{
    [Serializable]
    public class StatusEffect : Entity
    {
        [JsonIgnore] public int Duration { get { return this.duration; } }
        [JsonProperty] private readonly int duration = -1;
        [JsonIgnore] public EffectType Effect { get { return this.effect; } }
        [JsonProperty] private readonly EffectType effect = EffectType.Generic;
        [JsonIgnore] public int SpeedModifier { get { return this.speedModifier; } }
        [JsonProperty] private readonly int speedModifier = 0;
        [JsonIgnore] public int AttackModifier { get { return this.attackModifier; } }
        [JsonProperty] private readonly int attackModifier = 0;
        [JsonIgnore] public int DefenseModifier { get { return this.defenseModifier; } }
        [JsonProperty] private readonly int defenseModifier = 0;
        [JsonIgnore] public int FocusModifier { get { return this.focusModifier; } }
        [JsonProperty] private readonly int focusModifier = 0;
        [JsonIgnore] public int LuckModifier { get { return this.luckModifier; } }
        [JsonProperty] private readonly int luckModifier = 0;
        [JsonIgnore] public UntargetedGameAction OnUpkeep { get { return this.onUpkeep; } }
        [JsonProperty] private readonly UntargetedGameAction onUpkeep = null;
        [JsonIgnore] public UntargetedGameAction OnEOT { get { return this.onEOT; } }
        [JsonProperty] private readonly UntargetedGameAction onEOT = null;
    }
}
