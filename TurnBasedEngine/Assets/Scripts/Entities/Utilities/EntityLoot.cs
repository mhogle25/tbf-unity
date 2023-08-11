using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace BF2D.Game
{
    [Serializable]
    public class EntityLoot
    {
        [JsonIgnore] public string ID => this.id;
        [JsonProperty] private string id = string.Empty;
        [JsonIgnore] public int Probability => this.probability;
        [JsonProperty] private int probability = 100;
        [JsonIgnore] public int Count => this.count;
        [JsonProperty] private int count = 1;

        public void RollForLoot(List<string> collectionToAppend)
        {
            GameCtx ctx = GameCtx.One;

            CharacterStats luckiestPlayer = ctx.PartyLeader;
            foreach (CharacterStats player in ctx.ActivePlayers)
                if ((player.Luck > luckiestPlayer.Luck || luckiestPlayer.Dead) && !player.Dead)
                    luckiestPlayer = player;

            for (int i = 0; i < this.Count; i++)
                if (Utilities.Probability.Roll(luckiestPlayer, this.Probability))
                    collectionToAppend.Add(this.ID);
        }
    }
}