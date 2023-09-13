using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using BF2D.Game.Actions;
using UnityEngine;
using BF2D.Utilities;
using System.Linq;

namespace BF2D.Game
{
    [Serializable]
    public class EncounterFactory : Entity
    {
        [Serializable]
        private class Ranking<T>
        {
            [JsonProperty] public readonly int rank = 0;
            [JsonProperty] public readonly T property = default;
        };

        private struct PropertySegment<T>
        {
            public int index;
            public T property;
        }

        private class Selector<T>
        {
            [JsonProperty] protected readonly List<Ranking<T>> rankings = new();
            [JsonIgnore] private readonly List<T> propertiesCache = new();

            protected virtual void RefreshCache()
            {
                this.propertiesCache.Clear();

                foreach (Ranking<T> ranking in this.rankings)
                    for (int j = 0; j < ranking.rank; j++)
                        this.propertiesCache.Add(ranking.property);
            }

            public T Roll()
            {
                if (this.propertiesCache.Count < 1)
                    RefreshCache();

                int index = UnityEngine.Random.Range(0, this.propertiesCache.Count);
                return this.propertiesCache[index];
            }
        }

        [Serializable]
        private class MultiSelector<T> : Selector<T>
        {
            [JsonProperty] private readonly NumRandInt count = new(1);
            [JsonProperty] private readonly bool noOverlaps = false;

            [JsonIgnore] private readonly List<PropertySegment<T>> propertiesCache = new();

            protected override void RefreshCache()
            {
                this.propertiesCache.Clear();

                for (int i = 0; i < this.rankings.Count; i++)
                {
                    Ranking<T> ranking = this.rankings[i];
                    for (int j = 0; j < ranking.rank; j++)
                        this.propertiesCache.Add(new PropertySegment<T> { index = i, property = ranking.property });
                }
            }

            public IEnumerable<T> Roll(int maxCount = int.MaxValue)
            {
                int count = this.count.Calculate();

                if (count > maxCount)
                {
                    Debug.LogError("[EncounterFactory:Roll] Cannot exceed max count value");
                    yield break;
                }

                if (this.noOverlaps && count > this.rankings.Count)
                {
                    Debug.LogError("[EncounterFactory:Roll] Cannot prevent overlaps while also choosing more properties than this factory's ranking count");
                    yield break;
                }

                if (this.propertiesCache.Count < 1)
                    RefreshCache();

                HashSet<int> visited = new();
                for (int i = 0; i < count;)
                {
                    int index = UnityEngine.Random.Range(0, this.propertiesCache.Count);
                    PropertySegment<T> property = this.propertiesCache[index];

                    if (this.noOverlaps)
                    {
                        if (visited.Contains(property.index))
                            continue;
                        visited.Add(property.index);
                    }

                    yield return property.property;

                    i++;
                }
            }
        }

        [JsonIgnore] public override string ID { get => this.id; set => this.id = value; }
        [JsonIgnore] public string id = string.Empty;

        [JsonProperty] private readonly LootProperty loot = null;
        [JsonProperty] private readonly Selector<string> leader = new();
        [JsonProperty] private readonly MultiSelector<string> activeEnemies = new();
        [JsonProperty] private readonly MultiSelector<string> inactiveEnemies = new();
        [JsonProperty] private readonly TargetedGameAction onInit = null;

        [JsonIgnore] private readonly Stack<int> selectedPositions = new();

        public void StageEncounter()
        {
            GameCtx.One.StageEncounter(InstantiateEncounter());
        }

        public Encounter InstantiateEncounter() => new(this);

        public LootProperty ResolveLoot() => this.loot;

        public EncounterCharacterProperty ResolveLeader() => new()
        {
            ID = this.leader.Roll(),
            Position = RollPosition()
        };

        public List<EncounterCharacterProperty> ResolveActiveEnemies()
        {
            List<EncounterCharacterProperty> list = new();
            foreach (string id in this.activeEnemies.Roll(Numbers.maxPartySize))
            {
                list.Add(new EncounterCharacterProperty
                {
                    ID = id,
                    Position = RollPosition()
                });
            }
            return list;
        }

        public List<string> ResolveInactiveEnemies() => this.inactiveEnemies.Roll().ToList();

        public TargetedGameAction ResolveOnInit() => this.onInit;

        public void Reset()
        {
            this.selectedPositions.Clear();
            List<int> positions = new();
            for (int i = 0; i < Numbers.maxPartySize; i++)
                positions.Add(i);
            positions.Shuffle();
            foreach (int i in positions)
                this.selectedPositions.Push(i);
        }

        private int RollPosition()
        {
            if (this.selectedPositions.Count < 1)
            {
                Debug.Log("[EncounterFactory:RollPosition] Tried to roll a position but all grid positions were taken");
                return -1;
            }

            return this.selectedPositions.Pop();
        }
    }
}