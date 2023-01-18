using Newtonsoft.Json;
using BF2D.Enums;
using BF2D.Game.Actions;
using System;

namespace BF2D.Game
{
    [Serializable]
    public class StatusEffect : PersistentEffect
    {
        [JsonIgnore] public int Duration { get { return this.duration; } }
        [JsonProperty] private int duration = -1;

        public void Use()
        {
            this.duration--;
        }
    }
}
