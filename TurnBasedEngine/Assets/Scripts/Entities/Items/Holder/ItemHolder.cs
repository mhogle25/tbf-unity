using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace BF2D.Game
{
    [Serializable]
    public class ItemHolder : UtilityEntityHolder<ItemInfo>, IItemHolder
    {
        public void Destroy(ItemInfo info)
        {
            if (info is null)
            {
                Debug.LogError($"[ItemHolder:DestroyItem] Tried to remove an item from an item bag but the item info given was null");
                return;
            }

            if (!Contains(info))
            {
                Debug.LogError($"[ItemHolder:DestroyItem] Tried to remove an item from an item bag but the item info given wasn't in the bag");
                return;
            }

            if (info.Decrement() < 1)
            {
                RemoveAndForget(info);

                if (info.Generated)
                    GameCtx.Instance.DeleteItemIfCustom(info.ID);
            }
        }

        public void Destroy(string id) => Destroy(Get(id));

        public IEnumerable<ItemInfo> Useable => this.Where(info => info.Useable);
    }
}