using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace BF2D.Game
{
    [Serializable]
    public class LootProperty
    {
        [JsonConstructor]
        public LootProperty() { }

        private LootProperty(
            List<UtilityEntityLoot> itemLoot,
            List<UtilityEntityLoot> equipmentLoot,
            List<UtilityEntityLoot> gemLoot,
            List<UtilityEntityLoot> runeLoot,
            int currencyLoot,
            int etherLoot)
        {
            this.itemLoot = itemLoot;
            this.equipmentLoot = equipmentLoot;
            this.gemLoot = gemLoot;
            this.runeLoot = runeLoot;
            this.currencyLoot = currencyLoot;
            this.etherLoot = etherLoot;
        }

        // Loot
        [JsonIgnore] public IEnumerable<UtilityEntityLoot> ItemLoot => this.itemLoot;
        [JsonIgnore] public IEnumerable<UtilityEntityLoot> EquipmentLoot => this.equipmentLoot;
        [JsonIgnore] public IEnumerable<UtilityEntityLoot> GemLoot => this.gemLoot;
        [JsonIgnore] public IEnumerable<UtilityEntityLoot> RuneLoot => this.runeLoot;
        [JsonIgnore] public int CurrencyLoot => this.currencyLoot;
        [JsonIgnore] public int EtherLoot => this.etherLoot;

        [JsonProperty] private readonly List<UtilityEntityLoot> itemLoot = new();
        [JsonProperty] private readonly List<UtilityEntityLoot> equipmentLoot = new();
        [JsonProperty] private readonly List<UtilityEntityLoot> gemLoot = new();
        [JsonProperty] private readonly List<UtilityEntityLoot> runeLoot = new();
        [JsonProperty] private readonly int currencyLoot = 1;
        [JsonProperty] private readonly int etherLoot = 0;

        public static LootProperty operator+(LootProperty x, LootProperty y) =>
            new
            (
                itemLoot: AddLootTables(x.ItemLoot, y.ItemLoot),
                equipmentLoot: AddLootTables(x.EquipmentLoot, y.EquipmentLoot),
                gemLoot: AddLootTables(x.GemLoot, y.GemLoot),
                runeLoot: AddLootTables(x.RuneLoot, y.RuneLoot),
                currencyLoot: x.CurrencyLoot + y.CurrencyLoot,
                etherLoot: x.EtherLoot + y.EtherLoot
            );

        private static List<UtilityEntityLoot> AddLootTables(IEnumerable<UtilityEntityLoot> x, IEnumerable<UtilityEntityLoot> y)
        {
            List<UtilityEntityLoot> table = new(x);
            table.AddRange(new List<UtilityEntityLoot>(y));
            return table;
        }
    }
}