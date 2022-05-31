using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using BF2D.Game.Actions;

namespace BF2D.Game
{
    [Serializable]
    public class Item
    {
        [JsonIgnore] public string NameKey { get { return this.nameKey; } }
        [JsonProperty] private string nameKey = string.Empty;
        [JsonIgnore] public string IconKey { get { return this.iconKey; } }
        [JsonProperty] private string iconKey = string.Empty;
        [JsonIgnore] public bool Consumeable { get { return this.consumeable; } }
        [JsonProperty] private bool consumeable = true;
        [JsonIgnore] public string Description { get { return this.description; } }
        [JsonProperty] public string description = string.Empty;

        [JsonIgnore] public List<StatsAction> StatsActions { get { return this.statsActions; } }
        [JsonProperty] private List<StatsAction> statsActions = new List<StatsAction>();

        public void Use()
        {

        }

        private void InvokeStatsActions()
        {
            foreach (StatsAction action in this.statsActions)
            {

            }
        }
    }
}
