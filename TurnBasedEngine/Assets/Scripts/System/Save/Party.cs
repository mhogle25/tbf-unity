using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BF2D.Game.Actions;

namespace BF2D.Game
{
    [Serializable]
    public class Party
    {
        [JsonIgnore] public CharacterStats PartyLeader
        {
            get
            {
                CharacterStats smallestGridPosition = this.activeCharacters[0];
                foreach (CharacterStats character in this.activeCharacters)
                    if (character.GridPosition < smallestGridPosition.GridPosition)
                        smallestGridPosition = character;
                return smallestGridPosition;
            }
        }
        [JsonIgnore] public IEnumerable<CharacterStats> ActiveCharacters => this.activeCharacters;
        [JsonIgnore] public IEnumerable<CharacterStats> InactiveCharacters => this.inactiveCharacters;

        [JsonIgnore] public IItemHolder Items => this.items;
        [JsonIgnore] public IEquipmentHolder Equipments => this.equipments;
        [JsonIgnore] public ICharacterStatsActionHolder Gems => this.gems;
        [JsonIgnore] public IEquipModHolder Runes => this.runes;
        [JsonIgnore] public int Currency { get => this.currency; set => this.currency = value; }
        [JsonIgnore] public int Ether { get => this.ether; set => this.ether = value; }

        [JsonProperty] private readonly List<CharacterStats> activeCharacters = new();
        [JsonIgnore] private readonly Dictionary<string, CharacterStats> activeCharactersIndex = new();

        [JsonProperty] private readonly List<CharacterStats> inactiveCharacters = new();

        [JsonProperty] private readonly ItemHolder items = new();
        [JsonProperty] private readonly EquipmentHolder equipments = new();
        [JsonProperty] private readonly CharacterStatsActionHolder gems = new();
        [JsonProperty] private readonly EquipModHolder runes = new();
        [JsonProperty] private int currency = 0;
        [JsonProperty] private int ether = 0;

        public CharacterStats GetCharacter(string id)
        {
            if (this.activeCharactersIndex.Count < 1)
                foreach (CharacterStats character in this.activeCharacters)
                    this.activeCharactersIndex[character.ID] = character;

            if (!this.activeCharactersIndex.ContainsKey(id))
                return null;

            return this.activeCharactersIndex[id];
        }

        public void AddCharacter(CharacterStats newCharacter)
        {
            if (newCharacter is null)
            {
                Debug.LogError("[SaveData:AddPlayer] Tried to add a player but the player was null");
                return;
            }

            if (this.activeCharacters.Count >= Numbers.MaxPartySize)
            {
                Debug.LogError("[SaveData:AddPlayer] Tried to add a player but the maximum number of active players was reached.");
                return;
            }

            this.activeCharacters.Add(newCharacter);
            this.activeCharactersIndex[newCharacter.ID] = newCharacter;
        }
    }
}