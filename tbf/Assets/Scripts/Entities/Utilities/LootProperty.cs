using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace BF2D.Game
{
    [Serializable]
    public class LootProperty
    {
        // Loot
        [JsonIgnore] public IEnumerable<UtilityEntityLoot> ItemLoot => this.itemLoot;
        [JsonIgnore] public IEnumerable<UtilityEntityLoot> EquipmentLoot => this.equipmentLoot;
        [JsonIgnore] public IEnumerable<UtilityEntityLoot> GemLoot => this.gemLoot;
        [JsonIgnore] public IEnumerable<UtilityEntityLoot> RuneLoot => this.runeLoot;
        [JsonIgnore] public NumRandInt CurrencyLoot => this.currencyLoot;
        [JsonIgnore] public NumRandInt EtherLoot => this.etherLoot;

        [JsonProperty] private readonly List<UtilityEntityLoot> itemLoot = new();
        [JsonProperty] private readonly List<UtilityEntityLoot> equipmentLoot = new();
        [JsonProperty] private readonly List<UtilityEntityLoot> gemLoot = new();
        [JsonProperty] private readonly List<UtilityEntityLoot> runeLoot = new();
        [JsonProperty] private readonly NumRandInt currencyLoot = new(1);
        [JsonProperty] private readonly NumRandInt etherLoot = new(0);
    }
}