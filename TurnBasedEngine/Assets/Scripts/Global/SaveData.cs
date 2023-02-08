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

        [JsonIgnore] public List<CharacterStats> Players { get { return this.players; } }
        [JsonProperty] private readonly List<CharacterStats> players = new();

        public void AddPlayer(CharacterStats newPlayer)
        {
            if (newPlayer is null)
            {
                Debug.LogWarning("[SaveData] Tried to add a player but the player was null");
                return;
            }
            this.players.Add(newPlayer);
        }
    }
}
