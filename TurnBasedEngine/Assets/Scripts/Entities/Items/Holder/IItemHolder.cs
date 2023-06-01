using System.Collections.Generic;

namespace BF2D.Game
{
    public interface IItemHolder : IUtilityEntityHolder<ItemInfo>
    {
        /// <summary>
        /// Removes a single item from the holder. This method will delete the item's datafile if its count reaches zero and if it is a generated item. Use when customizing (transforming) or deleting an item.
        /// </summary>
        /// <param name="info">The item info to remove</param>
        public void Destroy(ItemInfo info);

        public IEnumerable<ItemInfo> Useable { get; }
    }
}