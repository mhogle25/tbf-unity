using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BF2D.Game
{
    [Serializable]
    public class Party
    {
        [JsonIgnore] public IEnumerable<CharacterStats> ActiveCharacters { get => this.activeCharacters; }
        [JsonProperty] private readonly List<CharacterStats> activeCharacters = new();

        [JsonIgnore] public IEnumerable<CharacterStats> InactiveCharacters { get => this.inactiveCharacters; }
        [JsonProperty] private readonly List<CharacterStats> inactiveCharacters = new();

        [JsonIgnore] public IItemHolder Bag { get => this.bag; }
        [JsonProperty] private readonly ItemHolder bag = new();

        [JsonIgnore] public int Currency { get => this.currency; set => this.currency = value; }
        [JsonProperty] private int currency = 0;

        [JsonIgnore] public int Ether { get => this.ether; set => this.ether = value; }
        [JsonProperty] private int ether = 0;

        public void AddCharacter(CharacterStats newCharacter)
        {
            if (newCharacter is null)
            {
                Terminal.IO.LogError("[SaveData:AddPlayer] Tried to add a player but the player was null");
                return;
            }

            if (this.activeCharacters.Count > Numbers.MaxPartySize)
            {
                Terminal.IO.LogError("[SaveData:AddPlayer] Tried to add a player but the maximum number of active players was reached.");
                return;
            }

            this.activeCharacters.Add(newCharacter);
        }
    }
}