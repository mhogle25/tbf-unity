using System;
using UnityEngine;
using BF2D.Game.Enums;
using BF2D.UI;
using TMPro;

namespace BF2D.Game.Combat
{
    public class EquippedListControl : OptionsGridControlInit
    {
        [Header("Equipped")]
        [SerializeField] private EquipCharacterTargeter equipCharacterTargeter = null;
        [SerializeField] private EquipmentBagControl bag = null;
        [SerializeField] private TextMeshProUGUI leftText = null;

        private readonly EquipmentTypeCollection<string> equipmentIconKeys = new()
        {
            [EquipmentType.Accessory] = "accessory",
            [EquipmentType.Head] = "head",
            [EquipmentType.Torso] = "torso",
            [EquipmentType.Legs] = "legs",
            [EquipmentType.Hands] = "hands",
            [EquipmentType.Feet] = "feet"
        };

        [SerializeField] private EquipmentType[] typeOrder =
        {
            EquipmentType.Accessory,
            EquipmentType.Head,
            EquipmentType.Torso,
            EquipmentType.Legs,
            EquipmentType.Hands,
            EquipmentType.Feet
        };

        public override void ControlInitialize()
        {
            OnNavigate(this.Controlled.GetSnapshot());
            base.ControlInitialize();
        }

        public override void ControlFinalize()
        {
            this.Controlled.SetCursorAtPosition(this.Controlled.CursorPosition, false);
            base.ControlFinalize();
        }

        public void SetupEquippedList(CharacterStats character)
        {
            EquipmentType[] types = Enum.GetValues(typeof(EquipmentType)) as EquipmentType[];
            for (int i = 0; i < types.Length; i++)
            {
                EquipmentType type = types[i];
                GridOption option = this.Controlled.At(new Vector2Int(0, i));
                Equipment equipped = character.GetEquipped(type);

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

        public void OnNavigate(OptionsGrid.Snapshot info)
        {
            if (!UICtx.One.IsControlling(this))
                return;

            EquipmentType type = this.typeOrder[info.cursorPosition1D];
            CharacterStats character = this.equipCharacterTargeter.Selected;

            this.bag.SetupEquipmentBag(character, type);
        }
    }
}