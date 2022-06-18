using BF2D.Game;
using BF2D.UI;
using System.Collections.Generic;

namespace BF2D.Combat
{
    public class StateHandleItem : CombatState
    {
        public Item CurrentItem { set { this.item = value; } }
        private Item item = null;
        public ItemInfo CurrentItemInfo { set { this.itemInfo = value; } }
        private ItemInfo itemInfo = null;
        public DialogTextboxControl TextboxControl { set { this.textboxControl = value; } }
        protected DialogTextboxControl textboxControl = null;

        public override void Begin()
        {
            if (this.item.Consumeable)
                this.itemInfo.Count--;
            SendMessage();
            this.finalize?.Invoke();
        }

        private void SendMessage()
        {
            List<string> dialog = new List<string>
            {
                $"{CombatManager.Instance.CurrentCharacter.Name} used {this.item.Name}."
            };

            if (this.item.UseMessage.Count > 0 || !(this.item.UseMessage is null))
            {
                dialog = this.item.UseMessage;
                dialog = this.textboxControl.Textbox.ReplaceInsertTags(dialog, new List<string>
                {
                    CombatManager.Instance.CurrentCharacter.Name,
                    this.item.Name
                });
            }

            this.textboxControl.Textbox.Dialog
            (
                dialog,
                0,
                () =>
                {

                }
            );

            UIControlsManager.Instance.TakeControl(this.textboxControl);
        }
    }
}