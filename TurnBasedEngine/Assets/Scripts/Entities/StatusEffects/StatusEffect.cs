using Newtonsoft.Json;
using NUnit.Framework.Internal;
using System;
using UnityEngine;

namespace BF2D.Game
{
    [Serializable]
    public class StatusEffect : PersistentEffect
    {
        [JsonIgnore] public int Duration => this.duration;
        [JsonIgnore] public bool Singleton => this.singleton;

        [JsonProperty] private readonly int duration = -1;
        [JsonProperty] private readonly bool singleton = false;

        public override string TextBreakdown(Equipment other, CharacterStats source)
        {
            return base.TextBreakdown(other, source) +
                $"-\nDuration: {this.Duration} turns\n" +
                (this.Singleton ? "Stackable\n" : "\n");
        }


    }
}
