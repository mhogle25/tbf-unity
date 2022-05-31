using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BF2D.UI;
using BF2D.Game;
using System;
using BF2D.Enums;
using BF2D.Game.Actions;

namespace BF2D.Combat
{
    public class BagMenuControl : UIOptionsControl
    {
        [SerializeField] private CombatManager combatManager = null;
        [SerializeField] private UIOptionsControl targetSelector = null;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Update()
        {
            base.Update();
        }

        public override void ControlInitialize()
        {
            base.ControlInitialize();
            LoadItems(this.combatManager.CurrentPlayer.Inventory);
        }

        public override void ControlFinalize()
        {
            base.ControlFinalize();
        }

        private void LoadItems(List<Item> items)
        {
            foreach (Item item in items)
            {
                this.optionsGrid.Add(new UIOptionData
                {
                    name = item.NameKey,
                    icon = GameInfo.Instance.GetIcon(item.IconKey),
                    actions = new InputButtonCollection<Action>
                    {
                        [InputButton.Confirm] = () =>
                        {
                            UIControlsManager.Instance.TakeControl(this.targetSelector);
                            ExecuteItemAction(item);
                        },
                        [InputButton.Back] = () =>
                        {
                            UIControlsManager.Instance.PassControlBack();
                        }
                    }
                });
            }
        }

        private void ExecuteItemAction(Item item)
        {
            SetupTargetSelector(item);
        }

        private void SetupTargetSelector(Item item)
        {
            foreach (StatsAction statsAction in item.StatsActions)
            {
                if (statsAction.MultiTarget)
                {

                } 
                else
                {

                }
            }
        }
    }
}
