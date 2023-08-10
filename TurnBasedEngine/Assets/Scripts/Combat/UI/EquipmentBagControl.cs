using System.Collections.Generic;
using UnityEngine;
using BF2D.UI;
using BF2D.Enums;
using System;
using TMPro;
using BF2D.Game.Enums;
using UnityEngine.Events;

namespace BF2D.Game.Combat
{
    public class EquipmentBagControl : OptionsGridControlPage
    {
        [Header("Equipment Bag")]
        [SerializeField] private InputEvents events = null;
        [SerializeField] private TextMeshProUGUI footer = null;
        [SerializeField] private TextMeshProUGUI rightText = null;

        public bool Initialized { get; private set; } = false;
        public Equipment Selected => this.currentContents[this.Controlled.CursorPosition1D].Get();

        private readonly List<EquipmentInfo> currentContents = new();

        public override void ControlInitialize()
        {
            base.ControlInitialize();
            OnNavigate(this.Controlled.GetSnapshot());
        }

        public override void ControlFinalize()
        {
            this.Controlled.SetCursorAtPosition(this.Controlled.CursorPosition, false);
            base.ControlFinalize();
        }

        public void SetupEquipmentBag(CharacterStats character, EquipmentType type)
        {
            this.currentContents.Clear();

            IEnumerable<EquipmentInfo> equipment = character.Equipment.FilterByType(type);

            UnityEvent backEvent = this.events.BackEvent;

            List<GridOption.Data> datas = new();
            foreach (EquipmentInfo info in equipment)
            {
                this.currentContents.Add(info);

                datas.Add(new GridOption.Data
                {
                    name = info.Name,
                    icon = info.GetIcon(),
                    text = info.Count.ToString(),
                    actions = new InputButtonCollection<Action>
                    {
                        [InputButton.Confirm] = this.events.ConfirmEvent.Invoke,
                        [InputButton.Back] = backEvent.Invoke
                    }
                });
            }

            this.Initialized = true;

            if (datas.Count < 1) { 
                datas.Add(new GridOption.Data
                {
                    name = "Empty Bag",
                    actions = new InputButtonCollection<Action>
                    {
                        [InputButton.Back] = backEvent.Invoke
                    }
                });

                this.Initialized = false;
            }
                    
            LoadOptions(datas);
        }

        public override void OnNavigate(OptionsGrid.Snapshot info)
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
