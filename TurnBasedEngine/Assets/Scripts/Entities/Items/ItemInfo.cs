using System;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;

namespace BF2D.Game
{
    [Serializable]
    public class ItemInfo : IUtilityEntityInfo
    {
        [JsonIgnore] public string ID => this.id; 
        [JsonProperty] private readonly string id = string.Empty;
        [JsonIgnore] public int Count => this.count; 
        [JsonProperty] private int count = 0;

        [JsonIgnore] private Item staged = null;

        [JsonIgnore] public Entity GetEntity => Get();

        [JsonIgnore] public IUtilityEntity GetUtility => Get();

        [JsonIgnore] public Sprite Icon => GameInfo.Instance.GetIcon(GetUtility.SpriteID);

        [JsonIgnore] public string Name => Get().Name;

        [JsonIgnore] public string Description => Get().Description;

        [JsonIgnore] public IEnumerable<Enums.AuraType> Auras => Get().Auras;

        [JsonIgnore] public bool Useable { get => Get().Useable; }

        [JsonIgnore] public bool CombatExclusive { get => Get().CombatExclusive; }

        [JsonConstructor]
        public ItemInfo() { }

        public ItemInfo(string id)
        {
            this.id = id;
        }

        public Item Get()
        {
            this.staged ??= GameInfo.Instance.InstantiateItem(this.id);
            return this.staged;
        }

        public void Increment()
        {
            this.count++;
        }

        public Item Use(IItemHolder owner)
        {
            Get();

            if (this.staged is null)
                return null;

            if (this.staged.Consumable)
            {
                this.count--;
                if (this.count < 1)
                    owner.RemoveItem(this);
            }

            return ResetStaged();
        }

        public Item ResetStaged()
        {
            Item item = this.staged;
            this.staged = null;
            return item;
        }
    }
}
