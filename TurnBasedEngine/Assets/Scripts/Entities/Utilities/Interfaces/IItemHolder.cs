using System.Collections.Generic;

namespace BF2D.Game
{
    public interface IItemHolder : IEnumerable<ItemInfo>
    {
        public ItemInfo AcquireItem(string id);

        public ItemInfo RemoveItem(ItemInfo info);
    }
}