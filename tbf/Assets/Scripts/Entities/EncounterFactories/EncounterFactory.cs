using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using BF2D.Game.Actions;
using UnityEngine;

namespace BF2D.Game
{
    [Serializable]
    public class EncounterFactory : Entity
    {
        [Serializable]
        private class Ranking
        {
            [JsonProperty] public readonly int rank = 0;
            [JsonProperty] public readonly string id = default;
        };

        private struct PropertySegment
        {
            public int index;
            public string id;
        }

        [Serializable]
        private class Selector : List<Ranking>
        {
            [JsonIgnore] private readonly List<string> propertiesCache = new();

            private void RefreshCache()
            {
                this.propertiesCache.Clear();

                foreach (Ranking ranking in this)
                    for (int j = 0; j < ranking.rank; j++)
                        this.propertiesCache.Add(ranking.id);
            }

            public string Roll()
            {
                if (this.Count < 1)
                    return string.Empty;

                if (this.propertiesCache.Count < 1)
                    RefreshCache();

                int index = UnityEngine.Random.Range(0, this.propertiesCache.Count);
                return this.propertiesCache[index];
            }
        }

        [Serializable]
        private class MultiSelector
        {
            [JsonProperty] private readonly List<Ranking> idRankings = new();
            [JsonProperty] private readonly NumRandInt count = new(0);
            [JsonProperty] private readonly bool noOverlap = false;

            [JsonIgnore] private readonly List<PropertySegment> propertiesCache = new();

            private void RefreshCache()
            {
                this.propertiesCache.Clear();

                for (int i = 0; i < this.idRankings.Count; i++)
                {
                    Ranking ranking = this.idRankings[i];
                    for (int j = 0; j < ranking.rank; j++)
                        this.propertiesCache.Add(new PropertySegment { index = i, id = ranking.id });
                }
            }

            public IEnumerable<string> Roll()
            {
                if (this.idRankings.Count < 1)
                    yield break;

                int count = this.count.Calculate();

                if (this.noOverlap && count > this.idRankings.Count)
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
                    PropertySegment property = this.propertiesCache[index];

                    if (this.noOverlap)
                    {
                        if (visited.Contains(property.index))
                            continue;
                        visited.Add(property.index);
                    }

                    yield return property.id;

                    i++;
                }

                yield break;
            }
        }

        [JsonIgnore] public override string ID { get => this.id; set => this.id = value; }
        [JsonIgnore] public string id = string.Empty;

        [JsonProperty] private readonly Selector openingDialogIDRankings = null;
        [JsonProperty] private readonly LootProperty loot = new();
        [JsonProperty] private readonly Selector leader = null;
        [JsonProperty] private readonly MultiSelector activeEnemies = new();
        [JsonProperty] private readonly MultiSelector inactiveEnemies = new();
        [JsonProperty] private readonly TargetedGameAction onInit = null;

        public void StageEncounter() => GameCtx.One.StageEncounter(InstantiateEncounter());

        public Encounter InstantiateEncounter()
        {
            Encounter encounter = new(this.openingDialogIDRankings?.Roll(), this.loot, this.onInit);
            string leaderID = this.leader.Roll();

            if (!string.IsNullOrEmpty(leaderID))
                encounter.AddEnemy(leaderID);

            foreach (string id in this.activeEnemies.Roll())
                encounter.AddEnemy(id);

            foreach (string id in this.inactiveEnemies.Roll())
                encounter.AddEnemy(id);

            return encounter;
        }
    }
}