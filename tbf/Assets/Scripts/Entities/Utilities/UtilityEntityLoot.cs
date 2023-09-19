using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BF2D.Game
{
    [Serializable]
    public class UtilityEntityLoot
    {
        [JsonProperty] private readonly string id = string.Empty;
        [JsonProperty] private readonly int probability = 100;
        [JsonProperty] private readonly int count = 1;

        [JsonIgnore] public string ID => this.id;
        [JsonIgnore] public int Probability => this.probability;
        [JsonIgnore] public int Count => this.count;

        public IEnumerable<string> RollForLoot()
        {
            GameCtx ctx = GameCtx.One;

            CharacterStats luckiestPlayer = ctx.PartyLeader;
            foreach (CharacterStats player in ctx.ActivePlayers)
                if ((player.Luck > luckiestPlayer.Luck || luckiestPlayer.Dead) && !player.Dead)
                    luckiestPlayer = player;

            for (int i = 0; i < this.Count; i++)
                if (Utilities.Probability.Roll(this.Probability, luckiestPlayer.Luck))
                    yield return this.ID;
        }
    }
}