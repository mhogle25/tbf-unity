using BF2D.Game.Actions;
using UnityEngine;

namespace BF2D.Game
{
    public class ItemCustomizer
    {
        public ItemCustomizer(ItemInfo itemInfo, IItemHolder itemOwner)
        {
            this.itemInfo = itemInfo;
            this.itemOwner = itemOwner;
        }

        private ItemInfo itemInfo = null;
        private readonly IItemHolder itemOwner = null;
        private int index = 0;

        public void SetIndex(int index) => this.index = index;

        public Utilities.FileWriter EmbueGem(CharacterActionInfo gemInfo, ICharacterActionHolder gemOwner, string newName)
        {
            Item newItem = GameCtx.One.InstantiateItem(this.itemInfo.ID).Setup<Item>(Strings.System.GenerateID(), newName);

            TargetedCharacterActionSlot[] slots = newItem.OnUse?.TargetedGemSlots;

            if (slots is null || slots.Length < 1)
            {
                Debug.LogError("[ItemCustomizer:EmbueGem] Tried to embue a gem to an item with no gem slots.");
                return null;
            }

            if (this.index < 0 || this.index >= slots.Length)
            {
                Debug.LogError("[ItemCustomizer:EmbueGem] Tried to embue a gem to an item in an invalid slot.");
                return null;
            }

            if (gemInfo.HasStatsUp && !newItem.Consumable)
            {
                Debug.LogError("[ItemCustomizer:EmbueGem] Cannot embue a stat modifier gem to a non-consumable item.");
                return null;
            }

            slots[this.index].ID = gemInfo.ID;

            return GameCtx.One.WriteItem(newItem, () =>
            {
                gemOwner.Extract(gemInfo);
                this.itemOwner.Destroy(this.itemInfo);
                this.itemInfo = this.itemOwner.Acquire(newItem.ID);
            });
        }
    }
}