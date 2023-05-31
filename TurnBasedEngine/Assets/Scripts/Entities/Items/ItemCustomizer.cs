using System.Collections;
using System.Collections.Generic;
using BF2D.Game.Actions;
using UnityEngine;
using BF2D.UI;
using System.Text.RegularExpressions;
using System;

namespace BF2D.Game
{
    public class ItemCustomizer
    {
        public ItemCustomizer(ItemInfo itemInfo, IItemHolder owner)
        {
            this.itemInfo = itemInfo;
            this.owner = owner;
        }

        private readonly ItemInfo itemInfo = null;
        private readonly IItemHolder owner = null;
        private int index = 0;

        public void SetIndex(int index) => this.index = index;

        public Utilities.FileWriter EmbueGem(string gemID, string newName)
        {
            string newItemID = Guid.NewGuid().ToString("N");
            Item newItem = GameInfo.Instance.InstantiateItem(this.itemInfo.ID).Setup<Item>(newItemID, newName);

            TargetedCharacterStatsAction[] targetedGems = newItem.OnUse?.TargetedGems;

            if (targetedGems is null || targetedGems.Length < 1)
                throw new Exception("[ItemCustomizer:EmbueGem] Tried to embue a gem to an item with no gem slots.");

            if (this.index < 0 || this.index >= targetedGems.Length)
                throw new Exception("[ItemCustomizer:EmbueGem] Tried to embue a gem to an item in an invalid slot.");

            targetedGems[this.index].SetGemID(gemID);

            return GameInfo.Instance.WriteItem(newItem, () =>
            {
                this.itemInfo.Decrement(this.owner);
                GameInfo.Instance.DeleteItemIfCustom(this.itemInfo.ID);
            });
        }
    }
}