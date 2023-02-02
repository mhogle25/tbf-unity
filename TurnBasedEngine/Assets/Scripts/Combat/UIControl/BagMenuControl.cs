using BF2D.Enums;
using BF2D.Game;
using BF2D.Game.Actions;
using BF2D.UI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace BF2D.Combat
{
    public class BagMenuControl : OptionsGridControl
    {
        [Header("Information")]
        [SerializeField] private TextMeshProUGUI nameText = null;
        [SerializeField] private TextMeshProUGUI descriptionText = null;

        private readonly List<Item> items = new();

        public override void ControlInitialize()
        {
            this.items.Clear();
            this.controlledOptionsGrid.Setup(this.controlledOptionsGrid.Width, this.controlledOptionsGrid.Height);

            foreach (ItemInfo itemInfo in CombatManager.Instance.CurrentCharacter.Stats.Items)
            {
                if (itemInfo.Get() is null)
                    return;

                this.items.Add(itemInfo.Get());

                this.controlledOptionsGrid.Add(new UIOption.Data
                {
                    name = itemInfo.Get().Name,
                    icon = GameInfo.Instance.GetIcon(itemInfo.Get().SpriteID),
                    text = itemInfo.Count.ToString(),
                    actions = new InputButtonCollection<Action>
                    {
                        [InputButton.Confirm] = () =>
                        {
                            CombatManager.Instance.SetupItemCombat(itemInfo);
                            this.controlledOptionsGrid.View.gameObject.SetActive(false);
                        },
                        [InputButton.Back] = () =>
                        {
                            UIControlsManager.Instance.PassControlBack();
                            this.controlledOptionsGrid.View.gameObject.SetActive(false);
                        }
                    }
                });
            }

            this.controlledOptionsGrid.SetCursorToFirst();
            OnNavigate(this.controlledOptionsGrid.CursorPosition1D);
            base.ControlInitialize();
        }

        public void OnNavigate(int index)
        {
            Item item = this.items[index];
            this.nameText.text = item.Name;
            this.descriptionText.text = $"{item.Description}\n";
            foreach(TargetedCharacterStatsAction statsAction in item.OnUse.TargetedGems)
            {
                this.descriptionText.text += statsAction.TextBreakdown(CombatManager.Instance.CurrentCharacter.Stats);
            }
        }
    }
}
