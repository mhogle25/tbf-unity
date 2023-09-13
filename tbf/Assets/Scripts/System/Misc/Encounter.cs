using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BF2D.Game.Actions;

namespace BF2D.Game
{
    [Serializable]
    public class Encounter : CharacterGroup, ICache
    {
        [JsonConstructor]
        public Encounter() { }

        public Encounter(EncounterFactory factory)
        {
            loot = factory.ResolveLoot();
            leader = factory.ResolveLeader();
            activeEnemies = factory.ResolveActiveEnemies();
            inactiveEnemies = factory.ResolveInactiveEnemies();
            onInit = factory.ResolveOnInit();

            factory.Reset();
        }

        [JsonProperty] private readonly LootProperty loot = new();

        [JsonProperty] private EncounterCharacterProperty leader = null;
        [JsonProperty] private readonly List<EncounterCharacterProperty> activeEnemies = new();
        [JsonProperty] private readonly List<string> inactiveEnemies = new();

        [JsonProperty] private readonly TargetedGameAction onInit = null;

        // Loot
        [JsonIgnore] public LootProperty Loot => this.loot;

        [JsonIgnore] public TargetedGameAction OnInit => this.onInit;

        [JsonIgnore] public override int ActiveCharacterCount => this.leader is null ? 0 : this.activeEnemies.Count + 1;
        [JsonIgnore] public override IEnumerable<ICharacterInfo> ActiveCharacters => this.leader is null ? null : new List<ICharacterInfo>() { this.leader }.Concat(this.activeEnemies);
        [JsonIgnore] public override ICharacterInfo Leader => this.leader;

        public void Clear()
        {
            foreach (EncounterCharacterProperty enemy in this.activeEnemies)
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

            EncounterCharacterProperty enemy = new()
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

            if (newLeader is not EncounterCharacterProperty leader)
            {
                Debug.LogError($"[Encounter:ChangeLeader] Tried to change leaders but the given character was not valid for an encounter -> {newLeader.Stats.Name}");
                return null;
            }

            if (!this.activeEnemies.Contains(leader))
            {
                Debug.LogError($"[Encounter:ChangeLeader] Tried to change leaders but the given character was not in the encounter -> {newLeader.Stats.Name}");
                return null;
            }

            EncounterCharacterProperty oldLeader = this.leader;
            this.activeEnemies.Remove(leader);
            this.activeEnemies.Add(oldLeader);
            this.leader = leader;
            return oldLeader;
        }
    }
}