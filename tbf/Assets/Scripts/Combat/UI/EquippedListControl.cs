using System;
using UnityEngine;
using BF2D.Game.Enums;
using BF2D.UI;
using TMPro;

namespace BF2D.Game.Combat
{
    public class EquippedListControl : OptionsGridControlInit
    {
        public struct Data
        {
            public Equipment equipment;
            public EquipmentType type;
        }

        [Header("Equipped")]
        [SerializeField] private EquipPlayerTargeter playerTargeter = null;
        [SerializeField] private EquipmentBagControl bag = null;
        [SerializeField] private TextMeshProUGUI leftText = null;

        public Data Selected => this.selected;
        private Data selected = new()
        {
            equipment = null,
            type = EquipmentType.Accessory
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
            base.ControlInitialize();
            this.Controlled.OnNavigate();
        }

        public override void ControlFinalize()
        {
            this.Controlled.SetCursorAtPosition(this.Controlled.CursorPosition, false);
            base.ControlFinalize();
        }

        public void Setup(CharacterStats character)
        {
            for (int i = 0; i < this.typeOrder.Length; i++)
            {
                EquipmentType type = this.typeOrder[i];
                GridOption option = this.Controlled.At(new Vector2Int(0, i));
                Equipment equipped = character.GetEquipped(type);

                GridOption.Data data = new()
                {
                    icon = equipped is null ? GameCtx.One.GetIcon(Strings.Equipment.GetTypeID(type)) : equipped.GetIcon(),
                    onNavigate = () => OnNavigate(type)
                };

                option.Setup(data);
            }
        }

        private void OnNavigate(EquipmentType type)
        {
            CharacterStats character = this.playerTargeter.Selected;

            Equipment equipped = character.GetEquipped(type);
            this.selected = new Data()
            {
                equipment = equipped,
                type = type
            };

            this.bag.Setup(character, type);
            this.bag.Controlled.OnNavigate();

            if (this.bag.Selected is not null)
                this.leftText.text = equipped?.TextBreakdown(this.bag.Selected, character) ?? GetUnequippedLabel(type);
            else
                this.leftText.text = equipped?.TextBreakdown(character) ?? GetUnequippedLabel(type);
        }

        private string GetUnequippedLabel(EquipmentType type) => $"{Strings.Equipment.GetType(type)} (unequipped)";
    }
}