using System;
using System.Collections.Generic;
using BF2D.Game.Enums;
using Newtonsoft.Json;
using Random = UnityEngine.Random;

namespace BF2D.Game.Actions
{
    public abstract class GameAction : ICombatAligned
    {
        [JsonProperty] protected List<string> message = null;

        [JsonProperty] protected bool messageRandom = false;
        [JsonProperty] protected float? messageDelayDuration = null;
        
        [JsonIgnore] public abstract bool IsRestoration { get; }
        [JsonIgnore] public abstract CombatAlignment Alignment { get; }
        
        [JsonIgnore] public bool MessageRandom => this.messageRandom;
        [JsonIgnore] public float? MessageDelayDuration => this.messageDelayDuration;
        
        public abstract string TextBreakdown(CharacterStats source);
        
        public List<string> GetMessage() => this.messageRandom ? 
            new() { this.message[Random.Range(0, this.message.Count)] } : 
            this.message;
    }
}