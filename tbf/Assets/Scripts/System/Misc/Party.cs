using System;
using System.Collections.Generic;
using BF2D.Game.Actions;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;

namespace BF2D.Game
{
    [Serializable]
    public class Party : CharacterGroup
    {
        [Serializable]
        private class CharacterProperty : ICharacterInfo
        {
            [JsonIgnore] public CharacterStats Stats { get => this.character; set => this.character = value; }
            [JsonIgnore] public int Position { get => this.position; set => this.position = value; }

            [JsonProperty] private int position;
            [JsonProperty] private CharacterStats character;
        }

        [JsonProperty] private readonly ItemHolder items = new();
        [JsonProperty] private readonly EquipmentHolder equipment = new();
        [JsonProperty] private readonly CharacterActionHolder gems = new();
        [JsonProperty] private readonly EquipModHolder runes = new();
        [JsonProperty] private int currency = 0;
        [JsonProperty] private int ether = 0;

        [JsonProperty] private CharacterProperty leader = null;
        [JsonProperty] private readonly List<CharacterProperty> activePlayers = new();
        [JsonProperty] private readonly List<CharacterStats> inactivePlayers = new();

        [JsonIgnore] public IItemHolder Items => this.items;
        [JsonIgnore] public IEquipmentHolder Equipment => this.equipment;
        [JsonIgnore] public ICharacterActionHolder Gems => this.gems;
        [JsonIgnore] public IEquipModHolder Runes => this.runes;
        [JsonIgnore] public int Currency { get => this.currency; set => this.currency = value; }
        [JsonIgnore] public int Ether { get => this.ether; set => this.ether = value; }

        [JsonIgnore] public override int ActiveCharacterCount => this.leader is null ? 0 : this.activePlayers.Count + 1;
        [JsonIgnore] public override IEnumerable<ICharacterInfo> ActiveCharacters => this.leader is null ? null : new ICharacterInfo[] { this.leader }.Concat(this.activePlayers);
        [JsonIgnore] public override ICharacterInfo Leader => this.leader;

        public ICharacterInfo AddPlayer(CharacterStats newCharacter)
        {
            if (newCharacter is null)
            {
                Debug.LogError("[SaveData:AddCharacter] Tried to add a character but the player was null");
                return null;
            }

            if (this.ActiveCharacterCount >= Numbers.maxPartySize)
            {
                this.inactivePlayers.Add(newCharacter);
                return null;
            }

            int position = GetNextAvailablePosition();

            CharacterProperty player = new()
            {
                Position = position,
                Stats = newCharacter
            };

            if (this.leader is null)
            {
                this.leader = player;
                return player;
            }

            this.activePlayers.Add(player);
            return player;
        }

        public override ICharacterInfo ChangeLeader(ICharacterInfo newLeader)
        {
            if (this.leader.Equals(newLeader))
                return newLeader;

            if (newLeader is not CharacterProperty leader)
            {
                Debug.LogError("[Party:ChangeLeader] Tried to change leaders but the given character was not valid for a party.");
                return null;
            }

            if (!this.activePlayers.Contains(leader))
            {
                Debug.LogError("[Party:ChangeLeader] Tried to change leaders but the given character was not in the party.");
                return null;
            }

            CharacterProperty oldLeader = this.leader;
            this.activePlayers.Remove(leader);
            this.activePlayers.Add(oldLeader);
            this.leader = leader;
            return oldLeader;
        }
    }
}