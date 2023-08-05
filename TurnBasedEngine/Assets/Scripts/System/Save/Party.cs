using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using BF2D.Game.Actions;
using System.Linq;

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
        [JsonIgnore] public CharacterStats[] ActiveCharacters => this.activeCharacters.ToArray();
        [JsonIgnore] public CharacterStats[] InactiveCharacters => this.inactiveCharacters.ToArray();

        [JsonIgnore] public IItemHolder Items => this.items;
        [JsonIgnore] public IEquipmentHolder Equipments => this.equipments;
        [JsonIgnore] public ICharacterStatsActionHolder Gems => this.gems;
        [JsonIgnore] public IEquipModHolder Runes => this.runes;
        [JsonIgnore] public int Currency { get => this.currency; set => this.currency = value; }
        [JsonIgnore] public int Ether { get => this.ether; set => this.ether = value; }

        [JsonProperty] private readonly List<CharacterStats> activeCharacters = new();
        [JsonProperty] private readonly List<CharacterStats> inactiveCharacters = new();

        [JsonProperty] private readonly ItemHolder items = new();
        [JsonProperty] private readonly EquipmentHolder equipments = new();
        [JsonProperty] private readonly CharacterStatsActionHolder gems = new();
        [JsonProperty] private readonly EquipModHolder runes = new();
        [JsonProperty] private int currency = 0;
        [JsonProperty] private int ether = 0;

        public void AddCharacter(CharacterStats newCharacter)
        {
            if (newCharacter is null)
            {
                Debug.LogError("[SaveData:AddCharacter] Tried to add a character but the player was null");
                return;
            }

            if (this.activeCharacters.Count >= Numbers.maxPartySize)
            {
                Debug.LogError("[SaveData:AddPlayer] Tried to add a character but the maximum number of active characters was reached.");
                return;
            }

            this.activeCharacters.Add(newCharacter);
        }
    }
}