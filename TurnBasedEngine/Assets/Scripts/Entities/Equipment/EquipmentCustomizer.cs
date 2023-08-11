using UnityEngine;

namespace BF2D.Game
{
    public class EquipmentCustomizer
    {
        public EquipmentCustomizer(EquipmentInfo equipmentInfo, IEquipmentHolder equipmentOwner)
        {
            this.equipmentInfo = equipmentInfo;
            this.equipmentOwner = equipmentOwner;
        }

        private EquipmentInfo equipmentInfo = null;
        private readonly IEquipmentHolder equipmentOwner = null;
        private int index = 0;

        public void SetIndex(int index) => this.index = index;

        public Utilities.FileWriter EmbueRune(EquipModInfo runeInfo, IEquipModHolder runeOwner, string newName)
        {
            Equipment newEquipment = GameCtx.One.InstantiateEquipment(this.equipmentInfo.ID).Setup<Equipment>(Strings.System.GenerateID(), newName);

            EquipModSlot[] slots = newEquipment.RuneSlots;

            if (slots is null || slots.Length < 1)
            {
                Debug.LogError("[EquipmentCustomizer:EmbueRune] Tried to embue a rune to an equipment with no rune slots.");
                return null;
            }

            if (this.index < 0 || this.index >= slots.Length)
            {
                Debug.LogError("[EquipmentCustomizer:EmbueRune] Tried to embue a rune to an equipment in an invalid slot.");
                return null;
            }

            slots[this.index].SetRuneID(runeInfo.ID);

            return GameCtx.One.WriteEquipment(newEquipment, () =>
            {
                runeOwner.Extract(runeInfo);
                this.equipmentOwner.Destroy(this.equipmentInfo);
                this.equipmentInfo = this.equipmentOwner.Acquire(newEquipment.ID);
            });
        }
    }
}