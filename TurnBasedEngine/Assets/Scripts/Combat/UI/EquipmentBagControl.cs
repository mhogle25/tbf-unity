using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BF2D.UI;
using System.Linq;
using BF2D.Enums;
using System;
using TMPro;

namespace BF2D.Game.Combat
{
    public class EquipmentBagControl : OptionsGridControlPage
    {
        [Header("Equipment Bag")]
        [SerializeField] private InputEvents events = null;
        [SerializeField] private TextMeshProUGUI footer = null;
        [SerializeField] private TextMeshProUGUI leftText = null;
        [SerializeField] private TextMeshProUGUI rightText = null;

        public void LoadOptionsFromInfos(IEnumerable<EquipmentInfo> equipments)
        {
            LoadOptions(equipments.Select(equipment =>
            new GridOption.Data
            {
                name = equipment.Name,
                icon = equipment.GetIcon(),
                actions = new InputButtonCollection<Action>
                {
                    [InputButton.Confirm] = this.events.ConfirmEvent.Invoke,
                    [InputButton.Back] = this.events.BackEvent.Invoke
                }
            }));
        }

        public override void OnNavigate(OptionsGrid.NavigateInfo info)
        {
            base.OnNavigate(info);

            char primary = this.PageOrientation == Axis.Horizontal ? Strings.System.RightArrowSymbol : Strings.System.DownArrowSymbol;
            char secondary = this.PageOrientation == Axis.Vertical ? Strings.System.LeftArrowSymbol : Strings.System.UpArrowSymbol;

            if (this.CurrentPage == 0)
                if (this.PageCount == 1)
                    this.footer.text = "";
                else 
                    this.footer.text = $"{primary}";
            else if (this.CurrentPage == this.PageCount - 1)
                this.footer.text = $"{secondary}";
            else
                this.footer.text = $"{secondary}{primary}";
        }
    }
}
