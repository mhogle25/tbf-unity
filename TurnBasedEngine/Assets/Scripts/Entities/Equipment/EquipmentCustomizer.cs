using BF2D.Game.Actions;
using UnityEngine;

namespace BF2D.Game
{
    public class EquipmentCustomizer
    {
        public EquipmentCustomizer(EquipmentInfo equipmentInfo, IEquipmentHolder owner)
        {
            this.equipmentInfo = equipmentInfo;
            this.owner = owner;
        }

        private EquipmentInfo equipmentInfo = null;
        private readonly IEquipmentHolder owner = null;
        private int index = 0;

        public void SetIndex(int index) => this.index = index;

        public Utilities.FileWriter EmbueRune(EquipModInfo runeInfo, IEquipModHolder runeOwner, string newName)
        {
            Equipment newEquipment = GameCtx.Instance.InstantiateEquipment(this.equipmentInfo.ID).Setup<Equipment>(Strings.System.GenerateID(), newName);

            EquipModWrapper[] slots = newEquipment.Runes;

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

            newEquipment.Runes[this.index].SetRuneID(runeInfo.ID);

            return GameCtx.Instance.WriteEquipment(newEquipment, () =>
            {
                runeOwner.Extract(runeInfo);
                this.owner.Destroy(this.equipmentInfo);
                this.equipmentInfo = this.owner.Acquire(newEquipment.ID);
            });
        }
    }
}