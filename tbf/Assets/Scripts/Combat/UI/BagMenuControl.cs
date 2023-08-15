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

        public override void ControlInitialize()
        {
            ClearOptions();
            this.Controlled.Setup();

            foreach (ItemInfo item in CombatCtx.One.CurrentCharacter.Stats.Items.Useable)
            {
                AddOption(new GridOption.Data
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
                });
            }

            if (!this.Armed)
            {
                AddOption(new GridOption.Data
                {
                    name = Strings.Game.BAG,
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

            RefreshGrid(0);

            this.Controlled.OnNavigate();

            base.ControlInitialize();
        }

        private void OnNavigate(Item item)
        {
            CharacterStats currentCharacter = CombatCtx.One.CurrentCharacter.Stats;

            if (currentCharacter.ItemsCount < 1)
            {
                this.nameText.text = Strings.Game.BAG;
                this.descriptionText.text = $"The {Strings.Game.BAG.ToLower()} is empty.";
                return;
            }

            this.nameText.text = item.Name;
            this.descriptionText.text = item.TextBreakdown(currentCharacter);
        }
    }
}
