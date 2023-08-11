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
                AddOption(ToGridOption(item));

            RefreshGrid(0);

            if (CombatCtx.One.CurrentCharacter.Stats.ItemsCount < 1)
            {
                this.Controlled.Add(new GridOption.Data
                {
                    name = Strings.Game.Bag,
                    text = "NULL",
                    onInput = new InputButtonCollection<Action>
                    {
                        [InputButton.Back] = () =>
                        {
                            UICtx.One.PassControlBack();
                            this.Controlled.View.gameObject.SetActive(false);
                        }
                    }
                });
            }

            this.Controlled.OnNavigate();

            base.ControlInitialize();
        }

        private void OnNavigate(Item item)
        {
            CharacterStats currentCharacter = CombatCtx.One.CurrentCharacter.Stats;

            if (currentCharacter.ItemsCount < 1)
            {
                this.nameText.text = Strings.Game.Bag;
                this.descriptionText.text = $"The {Strings.Game.Bag.ToLower()} is empty.";
                return;
            }

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
                onInput = new InputButtonCollection<Action>
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
                },
                onNavigate = () => OnNavigate(item.Get())
            };
        }
    }
}
