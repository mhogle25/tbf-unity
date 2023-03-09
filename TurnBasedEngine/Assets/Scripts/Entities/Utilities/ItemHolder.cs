using System.Collections.Generic;
using System;

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
                Terminal.IO.LogError($"[ItemHolder:AcquireItem] Tried to add an item with id {id} to an item bag but the item id given was invalid");
                return null;
            }

            info.Increment();
            return info;
        }

        public ItemInfo RemoveItem(ItemInfo info)
        {
            if (info is null)
            {
                Terminal.IO.LogError($"[ItemHolder:RemoveItem] Tried to remove an item from an item bag but the item info given was null");
                return null;
            }

            Remove(info);
            return info;
        }

        private ItemInfo AddItem(string id)
        {
            if (id == string.Empty)
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