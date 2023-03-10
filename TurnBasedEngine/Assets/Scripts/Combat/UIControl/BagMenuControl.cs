using BF2D.Enums;
using BF2D.Game.Actions;
using BF2D.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D.Game.Combat
{
    public class BagMenuControl : OptionsGridControlPage
    {
        [Header("Information")]
        [SerializeField] private Utilities.TextField nameText = null;
        [SerializeField] private Utilities.TextField descriptionText = null;

        private readonly List<Item> items = new();

        public override void ControlInitialize()
        {
            this.items.Clear();
            this.ClearOptions();
            this.controlledOptionsGrid.Setup(this.controlledOptionsGrid.Width, this.controlledOptionsGrid.Height);

            foreach (ItemInfo info in CombatManager.Instance.CurrentCharacter.Stats.Items)
            {
                this.items.Add(info.Get());
                AddOption(ToGridOption(info));
            }

            RefreshGrid(0);

            if (CombatManager.Instance.CurrentCharacter.Stats.ItemsCount < 1)
            {
                this.controlledOptionsGrid.Add(new GridOption.Data
                {
                    name = Strings.Game.Bag,
                    text = "NULL",
                    actions = new InputButtonCollection<Action>
                    {
                        [InputButton.Back] = () =>
                        {
                            UIControlsManager.Instance.PassControlBack();
                            this.controlledOptionsGrid.View.gameObject.SetActive(false);
                        }
                    }
                });
            }

            this.controlledOptionsGrid.SetCursorToFirst();
            OnNavigate(new OptionsGrid.NavigateInfo
            {
                cursorPosition1D = this.controlledOptionsGrid.CursorPosition1D
            });

            base.ControlInitialize();
        }

        public override void OnNavigate(OptionsGrid.NavigateInfo info)
        {
            base.OnNavigate(info);

            if (CombatManager.Instance.CurrentCharacter.Stats.ItemsCount < 1)
            {
                this.nameText.text = Strings.Game.Bag;
                this.descriptionText.text = $"The {Strings.Game.Bag.ToLower()} is empty.";
                return;
            }

            Item item = this.items[info.cursorPosition1D];
            this.nameText.text = item.Name;
            this.descriptionText.text = $"{item.Description}\n";
            foreach(TargetedCharacterStatsAction targetedGem in item.OnUse.TargetedGems)
            {
                this.descriptionText.text += "-\n" + targetedGem.Gem.TextBreakdown(CombatManager.Instance.CurrentCharacter.Stats);
            }
            this.descriptionText.text += "-\n";
        }

        private GridOption.Data ToGridOption(ItemInfo itemInfo)
        {
            return new GridOption.Data
            {
                name = itemInfo.Name,
                icon = itemInfo.Icon,
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
            };
        }
    }
}
