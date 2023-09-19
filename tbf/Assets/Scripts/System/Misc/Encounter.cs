using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using BF2D.Game.Actions;

namespace BF2D.Game
{
    public class Encounter : CharacterGroup, ICache
    {    
        private class CharacterProperty : ICharacterInfo, ICache
        {
            public CharacterStats Stats
            {
                get
                {
                    if (string.IsNullOrEmpty(this.ID))
                        return null;

                    this.cached ??= GameCtx.One.InstantiateEnemy(this.ID);

                    return this.cached;
                }
            }

            public string ID { private get; set; }
            public int Position { get; set; }
            public ICharacterController CurrentController { get; set; }

            private CharacterStats cached;

            public void Clear() => this.cached = null;
        }
        
        public Encounter(
            string openingDialogKey,
            LootProperty loot,
            TargetedGameAction onInit
            )
        {
            this.onInit = onInit;

            if (openingDialogKey is not null)
                this.openingDialogKey = openingDialogKey;

            if (loot is not null)
                this.loot = loot;
        }

        private readonly LootProperty loot = new();
        private readonly string openingDialogKey = $"di_opening_{Strings.System.DEFAULT_ID}"; 
        private readonly TargetedGameAction onInit = null;
        private CharacterProperty leader = null;
        private readonly List<CharacterProperty> activeEnemies = new();
        private readonly List<string> inactiveEnemies = new();

        public LootProperty Loot => this.loot;
        public TargetedGameAction OnInit => this.onInit;
        public string OpeningDialogKey => this.openingDialogKey;

        public override int ActiveCharacterCount => this.leader is null ? 0 : this.activeEnemies.Count + 1;
        public override IEnumerable<ICharacterInfo> ActiveCharacters
        {
            get
            {
                if (this.leader is null)
                    yield break;

                yield return this.leader;
                foreach (CharacterProperty enemy in this.activeEnemies)
                    yield return enemy;
            }
        }
        
        public override ICharacterInfo Leader => this.leader;

        public void Clear()
        {
            foreach (CharacterProperty enemy in this.activeEnemies)
                enemy.Clear();
        }

        public ICharacterInfo AddEnemy(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogError("[Encounter:AddEnemy] Tried to add an enemy but the id was null");
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

            CharacterProperty oldLeader = null;
            if (this.leader is not null)
            {
                oldLeader = this.leader;
                this.activeEnemies.Add(oldLeader);
            }
            this.activeEnemies.Remove(leader);
            this.leader = leader;
            return oldLeader;
        }
    }
}