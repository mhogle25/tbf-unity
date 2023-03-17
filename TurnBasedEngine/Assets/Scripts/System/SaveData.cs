using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BF2D.Game
{
    [Serializable]
    public class SaveData
    {
        [JsonIgnore] public string ID { get { return this.id; } set { this.id = value; } }
        [JsonProperty] private string id = string.Empty;

        [JsonIgnore] public IEnumerable<CharacterStats> ActivePlayers { get { return this.activePlayers; } }
        [JsonProperty] private readonly List<CharacterStats> activePlayers = new();

        [JsonIgnore] public IEnumerable<CharacterStats> InactivePlayers { get { return this.inactivePlayers; } }
        [JsonProperty] private readonly List<CharacterStats> inactivePlayers = new();

        [JsonIgnore] public IItemHolder Bag { get { return this.bag; } }
        [JsonProperty] private readonly ItemHolder bag = new();

        [JsonIgnore] public int Currency { get { return this.currency; } set { this.currency = value; } }
        [JsonProperty] private int currency = 0;

        public void AddPlayer(CharacterStats newPlayer)
        {
            if (newPlayer is null)
            {
                Terminal.IO.LogError("[SaveData:AddPlayer] Tried to add a player but the player was null");
                return;
            }

            if (this.activePlayers.Count > Numbers.MaxPartySize)
            {
                Terminal.IO.LogError("[SaveData:AddPlayer] Tried to add a player but the maximum number of active players was reached.");
                return;
            }

            this.activePlayers.Add(newPlayer);
        }
    }
}
