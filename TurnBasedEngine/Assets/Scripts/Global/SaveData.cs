using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace BF2D.Game
{
    [Serializable]
    public class SaveData
    {
        [JsonIgnore] public List<CharacterStats> Players { get { return this.players; } }
        [JsonProperty] private List<CharacterStats> players = new List<CharacterStats>();

        public void AddPlayer(CharacterStats newPlayer)
        {
            if (newPlayer is null)
            {
                Debug.LogWarning("[SaveData] Tried to add a player but the player was null");
                return;
            }
            players.Add(newPlayer);
        }
    }
}
