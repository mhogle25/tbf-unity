using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace BF2D.Game
{
    [Serializable]
    public class SaveData
    {
        [JsonIgnore] public string ID { get { return this.id; } set { this.id = value; } }
        [JsonProperty] private string id = string.Empty;

        [JsonIgnore] public List<CharacterStats> ActivePlayers { get { return this.activePlayers; } }
        [JsonProperty] private readonly List<CharacterStats> activePlayers = new();

        [JsonIgnore] public List<CharacterStats> InactivePlayers { get { return this.inactivePlayers; } }
        [JsonProperty] private readonly List<CharacterStats> inactivePlayers = new();

        public void AddPlayer(CharacterStats newPlayer)
        {
            if (newPlayer is null)
            {
                Terminal.IO.LogError("[SaveData:AddPlayer] Tried to add a player but the player was null");
                return;
            }

            if (activePlayers.Count > Macros.MaxPartySize)
            {
                Terminal.IO.LogError("[SaveData:AddPlayer] Tried to add a player but the maximum number of active players was reached.");
                return;
            }

            this.activePlayers.Add(newPlayer);
        }
    }
}
