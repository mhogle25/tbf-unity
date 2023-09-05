using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BF2D.Game
{
    [Serializable]
    public class Encounter : CharacterGroup, ICache
    {
        [Serializable]
        private class CharacterProperty : ICharacterInfo, ICache
        {
            [JsonIgnore] public CharacterStats Stats
            {
                get
                {
                    if (string.IsNullOrEmpty(this.id))
                        return null;

                    this.cached ??= GameCtx.One.InstantiateEnemy(this.id);

                    return this.cached;
                }
            }

            [JsonIgnore] public string ID { set => this.id = value; }
            [JsonIgnore] public int Position { get => this.position; set => this.position = value; }

            [JsonIgnore] private CharacterStats cached;

            [JsonProperty] private string id = string.Empty;
            [JsonProperty] private int position = 0;

            public void Clear() => this.cached = null;
        }

        [JsonProperty] private readonly LootProperty loot = new();

        [JsonProperty] private CharacterProperty leader = null;
        [JsonProperty] private readonly List<CharacterProperty> activeEnemies = new();
        [JsonProperty] private readonly List<string> inactiveEnemies = new();

        // Loot
        [JsonIgnore] public LootProperty Loot => this.loot;

        [JsonIgnore] public override int ActiveCharacterCount => this.leader is null ? 0 : this.activeEnemies.Count + 1;
        [JsonIgnore] public override IEnumerable<ICharacterInfo> ActiveCharacters => this.leader is null ? null : new List<ICharacterInfo>() { this.leader }.Concat(this.activeEnemies);
        [JsonIgnore] public override ICharacterInfo Leader => this.leader;

        public void Clear()
        {
            foreach (CharacterProperty enemy in this.activeEnemies)
                enemy.Clear();
        }

        public ICharacterInfo AddEnemy(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogError("[SaveData:AddCharacter] Tried to add a character but the id was null");
                return null;
            }

            if (this.ActiveCharacterCount >= Numbers.maxPartySize)
            {
                this.inactiveEnemies.Add(id);
                return null;
            }

            int position = GetNextAvailablePosition();

            CharacterProperty enemy = new()
            {
                ID = id,
                Position = position
            };

            if (this.leader is null)
            {
                this.leader = enemy;
                return enemy;
            }

            this.activeEnemies.Add(enemy);
            return enemy;
        }

        public override ICharacterInfo ChangeLeader(ICharacterInfo newLeader)
        {
            if (this.leader.Equals(newLeader))
                return newLeader;

            if (newLeader is not CharacterProperty leader)
            {
                Debug.LogError($"[Encounter:ChangeLeader] Tried to change leaders but the given character was not valid for an encounter -> {newLeader.Stats.Name}");
                return null;
            }

            if (!this.activeEnemies.Contains(leader))
            {
                Debug.LogError($"[Encounter:ChangeLeader] Tried to change leaders but the given character was not in the encounter -> {newLeader.Stats.Name}");
                return null;
            }

            CharacterProperty oldLeader = this.leader;
            this.activeEnemies.Remove(leader);
            this.activeEnemies.Add(oldLeader);
            this.leader = leader;
            return oldLeader;
        }
    }
}