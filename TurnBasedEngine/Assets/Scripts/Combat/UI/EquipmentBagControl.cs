using System.Collections.Generic;
using UnityEngine;
using BF2D.UI;
using BF2D.Enums;
using System;
using TMPro;
using BF2D.Game.Enums;

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

            if (!this.Armed) { 
                AddOption(new GridOption.Data
                {
                    name = "Empty Bag",
                    onInput = new InputButtonCollection<Action>
                    {
                        [InputButton.Back] = OnBack
                    },
                    onNavigate = () => OnNavigate(null)
                });
            }

            RefreshGrid(0);
        }

        private void OnNavigate(EquipmentInfo equipment)
        {
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

            if (equipment is not null)
            {
                this.selected = equipment;
                this.rightText.text = this.Selected.TextBreakdown(this.equipped.Selected, this.platforms.Selected);
            }
            else
            {
                this.rightText.text = $"(none)";
            }
        }

        private void OnBack()
        {
            UICtx.One.PassControlBack();
            this.textbox.SetViewActive(false);
        }

        private void EquipmentsOnConfirm()
        {
            this.textbox.AddResponseController(new ResponseYesNo(ResponseOnConfirm, OnBack));
            this.textbox.Dialog(this.textboxKey, 0, null, this.Selected.Name, CombatCtx.One.CurrentCharacter.Stats.Name);
            this.textbox.TakeControl();
        }

        private void ResponseOnConfirm(bool yes)
        {
            if (yes)
            {
                UICtx.One.ResetControlChain(false);
                CombatCtx.One.SetupEquipCombat(this.selected);
            }
            else
            {
                OnBack();
            }
        }
    }
}
