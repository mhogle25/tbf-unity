using System.Collections.Generic;
using UnityEngine;
using BF2D.UI;
using BF2D.Game;
using System;
using BF2D.Enums;
using TMPro;
using BF2D.Game.Actions;

namespace BF2D.Combat
{
    public class BagMenuControl : UIOptionsControl
    {
        [Header("Information")]
        [SerializeField] private TextMeshProUGUI nameText = null;
        [SerializeField] private TextMeshProUGUI descriptionText = null;

        private readonly List<Item> items = new List<Item>();

        public override void ControlInitialize()
        {
            this.items.Clear();
            this.optionsGrid.Setup(this.optionsGrid.Width, this.optionsGrid.Height);

            foreach (ItemInfo itemInfo in CombatManager.Instance.CurrentCharacter.Items)
            {
                this.items.Add(GameInfo.Instance.GetItem(itemInfo.Name));

                this.optionsGrid.Add(new UIOption.Data
                {
                    name = itemInfo.Name,
                    icon = GameInfo.Instance.GetIcon(itemInfo.Get().Icon),
                    text = itemInfo.Count.ToString(),
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
            OnNavigate(this.optionsGrid.OneDimensionalIndex);
            base.ControlInitialize();
        }

        public void OnNavigate(int index)
        {
            Item item = this.items[index];
            this.nameText.text = item.Name;
            this.descriptionText.text = $"{item.Description}\n";
            foreach(CharacterStatsAction statsAction in item.OnUse.StatsActions)
            {
                this.descriptionText.text += statsAction.TextBreakdown(CombatManager.Instance.CurrentCharacter);
            }
        }
    }
}
