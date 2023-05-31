using System.Collections.Generic;

namespace BF2D.Game
{
    public interface IItemHolder : IEnumerable<ItemInfo>
    {
        public ItemInfo AcquireItem(string id);

        public ItemInfo RemoveItem(ItemInfo info);

        public ItemInfo TransferItem(ItemInfo info, IItemHolder reciever);

        public ItemInfo GetItem(string id);

        public bool HasItem(string id);

        public IEnumerable<ItemInfo> Useable { get; }
    }
}