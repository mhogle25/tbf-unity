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
            this.Controlled.Setup(this.Controlled.Width, this.Controlled.Height);

            foreach (ItemInfo item in CombatCtx.One.CurrentCharacter.Stats.Items.Useable)
            {
                this.items.Add(item.Get());
                AddOption(ToGridOption(item));
            }

            RefreshGrid(0);

            if (CombatCtx.One.CurrentCharacter.Stats.ItemsCount < 1)
            {
                this.Controlled.Add(new GridOption.Data
                {
                    name = Strings.Game.Bag,
                    text = "NULL",
                    actions = new InputButtonCollection<Action>
                    {
                        [InputButton.Back] = () =>
                        {
                            UICtx.One.PassControlBack();
                            this.Controlled.View.gameObject.SetActive(false);
                        }
                    }
                });
            }

            OnNavigate(new OptionsGrid.NavigateInfo
            {
                cursorPosition1D = this.Controlled.CursorPosition1D
            });

            base.ControlInitialize();
        }

        public override void OnNavigate(OptionsGrid.NavigateInfo info)
        {
            base.OnNavigate(info);

            CharacterStats currentCharacter = CombatCtx.One.CurrentCharacter.Stats;

            if (currentCharacter.ItemsCount < 1)
            {
                this.nameText.text = Strings.Game.Bag;
                this.descriptionText.text = $"The {Strings.Game.Bag.ToLower()} is empty.";
                return;
            }

            Item item = this.items[info.cursorPosition1D + this.Controlled.Size * this.CurrentPage];
            this.nameText.text = item.Name;
            this.descriptionText.text = item.TextBreakdown(currentCharacter);
        }

        private GridOption.Data ToGridOption(ItemInfo item)
        {
            return new GridOption.Data
            {
                name = item.Name,
                icon = item.Icon,
                text = item.Count.ToString(),
                actions = new InputButtonCollection<Action>
                {
                    [InputButton.Confirm] = () =>
                    {
                        CombatCtx.One.SetupItemCombat(item);
                        this.Controlled.View.gameObject.SetActive(false);
                    },
                    [InputButton.Back] = () =>
                    {
                        UICtx.One.PassControlBack();
                        this.Controlled.View.gameObject.SetActive(false);
                    }
                }
            };
        }
    }
}
