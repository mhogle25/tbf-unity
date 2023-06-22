using BF2D.Game.Actions;
using UnityEngine;

namespace BF2D.Game
{
    public class ItemCustomizer
    {
        public ItemCustomizer(ItemInfo itemInfo, IItemHolder owner)
        {
            this.itemInfo = itemInfo;
            this.owner = owner;
        }

        private ItemInfo itemInfo = null;
        private readonly IItemHolder owner = null;
        private int index = 0;

        public void SetIndex(int index) => this.index = index;

        public Utilities.FileWriter EmbueGem(CharacterStatsActionInfo gem, ICharacterStatsActionHolder gemOwner, string newName)
        {
            Item newItem = GameCtx.Instance.InstantiateItem(this.itemInfo.ID).Setup<Item>(Strings.System.GenerateID(), newName);

            TargetedCharacterStatsAction[] targetedGems = newItem.OnUse?.TargetedGemSlots;

            if (targetedGems is null || targetedGems.Length < 1)
            {
                Debug.LogError("[ItemCustomizer:EmbueGem] Tried to embue a gem to an item with no gem slots.");
                return null;
            }

            if (this.index < 0 || this.index >= targetedGems.Length)
            {
                Debug.LogError("[ItemCustomizer:EmbueGem] Tried to embue a gem to an item in an invalid slot.");
                return null;
            }

            if (gem.HasStatsUp && !newItem.Consumable)
            {
                Debug.LogError("[ItemCustomizer:EmbueGem] Cannot embue a stat modifier gem to a non-consumable item.");
                return null;
            }

            targetedGems[this.index].SetGemID(gem.ID);

            return GameCtx.Instance.WriteItem(newItem, () =>
            {
                gemOwner.Extract(gem);
                this.owner.Destroy(this.itemInfo);
                this.itemInfo = this.owner.Acquire(newItem.ID);
            });
        }
    }
}