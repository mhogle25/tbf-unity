using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BF2D.Game
{
    [Serializable]
    public class SaveData
    {
        [JsonIgnore] public string ID { get => this.id; set => this.id = value; }
        [JsonProperty] private string id = string.Empty;

        [JsonIgnore] public IEnumerable<CharacterStats> ActivePlayers { get => this.activePlayers; }
        [JsonProperty] private readonly List<CharacterStats> activePlayers = new();

        [JsonIgnore] public IEnumerable<CharacterStats> InactivePlayers { get => this.inactivePlayers;  }
        [JsonProperty] private readonly List<CharacterStats> inactivePlayers = new();

        [JsonIgnore] public IItemHolder Bag { get => this.bag; }
        [JsonProperty] private readonly ItemHolder bag = new();

        [JsonIgnore] public int Currency { get => this.currency; set => this.currency = value; }
        [JsonProperty] private int currency = 0;

        [JsonIgnore] public int Ether { get => this.ether; set => this.ether = value; }
        [JsonProperty] private int ether = 0;

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
