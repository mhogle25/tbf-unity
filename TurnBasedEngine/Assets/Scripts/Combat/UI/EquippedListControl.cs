using System;
using UnityEngine;
using BF2D.Game.Enums;
using BF2D.UI;

namespace BF2D.Game.Combat
{
    public class EquippedListControl : OptionsGridControlInit
    {
        private readonly EquipmentTypeCollection<string> equipmentIconKeys = new()
        {
            [EquipmentType.Accessory] = "accessory",
            [EquipmentType.Head] = "head",
            [EquipmentType.Torso] = "torso",
            [EquipmentType.Legs] = "legs",
            [EquipmentType.Hands] = "hands",
            [EquipmentType.Feet] = "feet"
        };

        public override void ControlInitialize()
        {
            base.ControlInitialize();
        }

        public override void ControlFinalize()
        {
            this.Controlled.SetCursorAtPosition(this.Controlled.CursorPosition, false);
            base.ControlFinalize();
        }

        public void SetupEquippedList(CharacterStats stats)
        {
            EquipmentType[] types = Enum.GetValues(typeof(EquipmentType)) as EquipmentType[];
            for (int i = 0; i < types.Length; i++)
            {
                EquipmentType type = types[i];
                GridOption option = this.Controlled.At(new Vector2Int(0, i));
                Equipment equipped = stats.GetEquipped(type);

                GridOption.Data data = equipped is null ?
                new GridOption.Data
                {
                    icon = GameCtx.One.GetIcon(this.equipmentIconKeys[type]),
                } :
                new GridOption.Data
                {
                    icon = equipped.GetIcon()
                };

                option.Setup(data);
            }
        }
    }
}