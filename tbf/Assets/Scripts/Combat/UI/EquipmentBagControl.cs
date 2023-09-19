using System.Collections.Generic;
using UnityEngine;
using BF2D.UI;
using BF2D.Enums;
using System;
using TMPro;
using BF2D.Game.Enums;
using System.Linq;

namespace BF2D.Game.Combat
{
    public class EquipmentBagControl : OptionsGridControlPage
    {
        [Header("Equipment Bag")]
        [SerializeField] private EquipPlayerTargeter platforms = null;
        [SerializeField] private EquippedListControl equipped = null;
        [Header("EB - Textbox")]
        [SerializeField] private DialogTextboxControl textbox = null;
        [SerializeField] private string textboxKey = "di_equip_bag";
        [Header("EB - Text Fields")]
        [SerializeField] private TextMeshProUGUI footer = null;
        [SerializeField] private TextMeshProUGUI rightText = null;

        public Equipment Selected => this.selected?.Get();
        private EquipmentInfo selected = null;

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

        public void Setup(CharacterStats character, EquipmentType type)
        {
            IEnumerable<EquipmentInfo> equipment = character.Equipment.FilterByType(type);

            ClearOptions();
            this.Controlled.Setup();

            bool emptyBag = !equipment.Any();
            bool equipped = this.equipped.Selected.equipment is not null;

            if (equipped || emptyBag)
            {
                AddOption(new GridOption.Data
                {
                    name = emptyBag && !equipped ? $"No {Strings.Equipment.GetType(type)}" : $"Unequip {Strings.Equipment.GetType(type)}",
                    icon = emptyBag && !equipped ? null : GameCtx.One.GetIcon(Strings.Equipment.GetTypeID(type)),
                    text = string.Empty,
                    onInput = new InputButtonCollection<Action>
                    {
                        [InputButton.Confirm] = EquipmentsOnConfirm,
                        [InputButton.Back] = OnBack
                    },
                    onNavigate = () => OnNavigate(null)
                });
            }

            foreach (EquipmentInfo info in equipment)
            {
                AddOption(new GridOption.Data
                {
                    name = info.Name,
                    icon = info.GetIcon(),
                    text = info.Count.ToString(),
                    onInput = new InputButtonCollection<Action>
                    {
                        [InputButton.Confirm] = EquipmentsOnConfirm,
                        [InputButton.Back] = OnBack
                    },
                    onNavigate = () => OnNavigate(info)
                });
            }

            RefreshGrid(0);
        }

        private void OnNavigate(EquipmentInfo equipment)
        {
            this.selected = equipment;

            char primary = this.PageOrientation == Axis.Horizontal ? Strings.System.RIGHT_ARROW_SYMBOL : Strings.System.DOWN_ARROW_SYMBOL;
            char secondary = this.PageOrientation == Axis.Vertical ? Strings.System.LEFT_ARROW_SYMBOL : Strings.System.UP_ARROW_SYMBOL;

            if (this.CurrentPage == 0)
                if (this.PageCount == 1)
                    this.footer.text = "";
                else
                    this.footer.text = $"{primary}";
            else if (this.CurrentPage == this.PageCount - 1)
                this.footer.text = $"{secondary}";
            else
                this.footer.text = $"{secondary}{primary}";

            if (this.equipped.Selected.equipment is null && equipment is null)
            {
                this.rightText.text = "(none)";
            }
            else if (equipment is null)
            {
                this.rightText.text = $"Unequip {this.equipped.Selected.equipment.Name}";
            }
            else
            {
                this.rightText.text = this.Selected.TextBreakdown(this.equipped.Selected.equipment, this.platforms.Selected);
            }
        }

        private static void OnBack() =>
            UICtx.One.PassControlBack();

        private void EquipmentsOnConfirm()
        {

            int dialogIndex = 0;
            if (this.Selected is null)
            {
                if (this.equipped.Selected.equipment is null)
                    return;

                dialogIndex = 1;
            }

            this.textbox.AddResponseController(new ResponseControllerIndexed(ResponseOnConfirm, OnBack));
            this.textbox.Dialog(
                this.textboxKey,
                dialogIndex,
                null,
                this.Selected is null ? this.equipped.Selected.equipment.Name : this.Selected.Name,
                CombatCtx.One.CurrentCharacter.Stats.Name
                );
            this.textbox.TakeControl();
        }

        private void ResponseOnConfirm(string yesOrNo)
        {
            if (!YesNo.Either(yesOrNo))
                return;

            if (YesNo.Yes(yesOrNo))
            {
                UICtx.One.ResetControlChain(false);
                CombatCtx.One.SetupEquipCombat(this.selected, this.equipped.Selected.type);
            }
            else
            {
                OnBack();
            }
        }
    }
}
