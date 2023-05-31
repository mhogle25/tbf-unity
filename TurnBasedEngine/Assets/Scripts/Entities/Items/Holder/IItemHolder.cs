using System.Collections.Generic;

namespace BF2D.Game
{
    public interface IItemHolder : IEnumerable<ItemInfo>
    {
        public ItemInfo AcquireItem(string id);

        /// <summary>
        /// Removes a single item from the holder. This method will delete the item's datafile if its count reaches zero and if it is a generated item. Use when customizing (transforming) or deleting an item.
        /// </summary>
        /// <param name="info">The item info to remove</param>
        public void RemoveItem(ItemInfo info);

        /// <summary>
        /// Moves an item from one holder to another
        /// </summary>
        /// <param name="info">The item info to move</param>
        /// <param name="receiver">The receiving holder</param>
        /// <returns>The recieved item info</returns>
        public ItemInfo TransferItem(ItemInfo info, IItemHolder receiver);

        public ItemInfo GetItem(string id);

        public IEnumerable<ItemInfo> Useable { get; }
    }
}