using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json;
using UnityEngine;

namespace BF2D.Game.Actions
{
    public abstract class GameAction
    {
        [JsonIgnore] public List<string> Message
        {
            get 
            {
                if (this.messageRandom)
                    return new List<string>() { this.message[Random.Range(0, this.message.Count)] };

                return this.message;
            }
        }
        [JsonProperty] protected List<string> message = new();

        [JsonProperty] protected readonly bool messageRandom = false;

        public abstract string TextBreakdown(CharacterStats source);
    }
}