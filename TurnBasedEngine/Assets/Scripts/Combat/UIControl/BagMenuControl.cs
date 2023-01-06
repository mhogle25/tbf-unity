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
        [SerializeField] private UnityEvent<ItemInfo> onSelectItem;

        private readonly List<Item> items = new();

        public override void ControlInitialize()
        {
            this.items.Clear();
            this.controlledOptionsGrid.Setup(this.controlledOptionsGrid.Width, this.controlledOptionsGrid.Height);

            foreach (ItemInfo itemInfo in CombatManager.Instance.CurrentCharacter.Stats.Items)
            {
                this.items.Add(GameInfo.Instance.GetItem(itemInfo.Name));

                this.controlledOptionsGrid.Add(new UIOption.Data
                {
                    name = itemInfo.Name,
                    icon = GameInfo.Instance.GetIcon(itemInfo.Get().Icon),
                    text = itemInfo.Count.ToString(),
                    actions = new InputButtonCollection<Action>
                    {
                        [InputButton.Confirm] = () =>
                        {
                            this.onSelectItem.Invoke(itemInfo);
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

            this.controlledOptionsGrid.SetCursorAtHead();
            OnNavigate(this.controlledOptionsGrid.CursorPosition1D);
            base.ControlInitialize();
        }

        public void OnNavigate(int index)
        {
            Item item = this.items[index];
            this.nameText.text = item.Name;
            this.descriptionText.text = $"{item.Description}\n";
            foreach(CharacterStatsAction statsAction in item.OnUse.StatsActions)
            {
                this.descriptionText.text += statsAction.TextBreakdown(CombatManager.Instance.CurrentCharacter.Stats);
            }
        }
    }
}
