using UnityEngine;
using BF2D.Game;
using BF2D.Game.Actions;
using BF2D.Enums;
using System;
using BF2D.UI;
using System.Collections.Generic;

namespace BF2D.Combat
{
    public abstract class CharacterCombat : MonoBehaviour
    {
        public class StateData
        {
            public DialogTextboxControl textboxControl = null;
            public GameAction gameAction = null;
            public List<string> dialog = null;
        }

        public class ItemStateData : StateData
        {
            public Item item = null;
            public ItemInfo itemInfo = null;
        }

        public abstract CharacterType Type { get; }

        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected Animator animator;

        protected StateData state = null;

        private void Awake()
        {
            this.animator.Play(BF2D.Game.Strings.Animation.Entry, -1);
        }

        #region Public Methods
        public void EndCombatAction()
        {
            this.state.textboxControl.Textbox.Dialog(this.state.dialog, 0, () =>
            {

            });
            UIControlsManager.Instance.TakeControl(this.state.textboxControl);
            this.state = null;
        }

        public void ExecuteCombatAction(ItemStateData itemStateData)
        {
            if (itemStateData.item.Consumeable)
                itemStateData.itemInfo.Count--;

            this.state = itemStateData;

            SendUseMessage(itemStateData.item.Name, itemStateData.item.UseMessage);
        }

        public void TriggerAnimationSwap()
        {
            this.animator.SetTrigger(BF2D.Game.Strings.Animation.Swap);
        }

        public abstract void TriggerGameAction();
        #endregion

        #region Private Methods
        private void SendUseMessage(string subjectName, List<string> useMessage)
        {
            List<string> dialog = new()
            {
                $"{CombatManager.Instance.CurrentCharacter.Name} used {subjectName}."
            };

            if (useMessage.Count > 0 || useMessage is not null)
            {
                dialog = useMessage;
                dialog = DialogTextbox.ReplaceInsertTags(dialog, new List<string>
                {
                    CombatManager.Instance.CurrentCharacter.Name,
                    subjectName
                });
            }

            this.state.textboxControl.Textbox.Dialog
            (
                dialog,
                0,
                () =>
                {
                    TriggerAnimationSwap();
                }
            );

            UIControlsManager.Instance.TakeControl(this.state.textboxControl);
        }
        #endregion
    }
}
