using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace BF2D.Game
{
    [Serializable]
    public class ItemHolder : List<ItemInfo>, IItemHolder
    {
        public ItemInfo AcquireItem(string id)
        {
            ItemInfo info = AddItem(id);

            if (info is null)
            {
                Debug.LogError($"[ItemHolder:AcquireItem] Tried to add an item with id {id} to an item bag but the item id given was invalid");
                return null;
            }

            info.Increment();
            return info;
        }

        public void RemoveItem(ItemInfo info)
        {
            if (info is null)
            {
                Debug.LogError($"[ItemHolder:RemoveItem] Tried to remove an item from an item bag but the item info given was null");
                return;
            }

            if (!Contains(info))
            {
                Debug.LogError($"[ItemHolder:RemoveItem] Tried to remove an item from an item bag but the item info given wasn't in the bag");
                return;
            }

            if (info.Decrement() < 1)
            {
                Remove(info);

                if (info.Generated)
                    GameCtx.Instance.DeleteItemIfCustom(info.ID);
            }
        }

        public ItemInfo TransferItem(ItemInfo info, IItemHolder receiver)
        {
            if (info is null)
            {
                Debug.LogError($"[ItemHolder:TransferItem] Tried to transfer an item from an item bag but the item info given was null");
                return null;
            }

            if (!Contains(info))
            {
                Debug.LogError($"[ItemHolder:TransferItem] Tried to transfer an item from an item bag but the item info given wasn't in the bag");
                return null;
            }

            if (info.Decrement() < 1)
                Remove(info);

            return receiver.AcquireItem(info.ID); 
        }

        public IEnumerable<ItemInfo> Useable => this.Where(info => info.Useable);

        public ItemInfo GetItem(string id)
        {
            foreach (ItemInfo info in this)
            {
                if (info.ID == id)
                    return info;
            }

            return null;
        }

        private ItemInfo AddItem(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            foreach (ItemInfo info in this)
            {
                if (info.ID == id)
                    return info;
            }

            ItemInfo newInfo = new(id);
            Add(newInfo);
            return newInfo;
        }
    }
}