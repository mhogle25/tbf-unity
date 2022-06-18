using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BF2D.UI;
using BF2D.Game;
using System;
using BF2D.Enums;
using BF2D.Actions;

namespace BF2D.Combat
{
    public class BagMenuControl : UIOptionsControl
    {

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
            Debug.Log(CombatManager.Instance.CurrentCharacter);
            Debug.Log("Made it");

            this.optionsGrid.Setup(this.optionsGrid.Width, this.optionsGrid.Height);

            foreach (ItemInfo itemInfo in CombatManager.Instance.CurrentCharacter.Items)
            {
                Debug.Log("Inside the loop");
                
                this.optionsGrid.Add(new UIOption.Data
                {
                    name = itemInfo.Name,
                    icon = GameInfo.Instance.GetIcon(itemInfo.Get().Icon),
                    actions = new InputButtonCollection<Action>
                    {
                        [InputButton.Confirm] = () =>
                        {
                            CombatManager.Instance.ExecuteItem(itemInfo);
                            UIControlsManager.Instance.ResetControlChain(false);
                        },
                        [InputButton.Back] = () =>
                        {
                            UIControlsManager.Instance.PassControlBack();
                            this.optionsGrid.View.gameObject.SetActive(false);
                        }
                    }
                });
            }

            this.optionsGrid.SetCursorAtHead();

            base.ControlInitialize();
        }

        public override void ControlFinalize()
        {
            base.ControlFinalize();
        }
    }
}
