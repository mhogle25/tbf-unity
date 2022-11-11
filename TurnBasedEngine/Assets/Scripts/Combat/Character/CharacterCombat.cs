using UnityEngine;
using BF2D.Game;
using BF2D.Game.Actions;
using BF2D.Enums;
using System;
using System.IO;
using BF2D.UI;
using System.Collections.Generic;
using UnityEditor;

namespace BF2D.Combat
{
    public abstract class CharacterCombat : MonoBehaviour
    {
        public abstract CharacterType Type { get; }

        public DialogTextboxControl TextboxControl { get { return this.textboxControl; } set { this.textboxControl = value; } }
        [SerializeField] private DialogTextboxControl textboxControl = null;

        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected Animator animator;

        private readonly Queue<CharacterStatsAction> incomingStatsActions = new();

        private void Awake()
        {
            this.animator.Play(BF2D.Game.Strings.Animation.Entry, -1);
        }

        #region Public Methods
        public void ExecuteCombatAction(Item item, ItemInfo itemInfo)
        {
            if (item.Consumeable)
                itemInfo.Count--;

            SendUseMessage(item.Name, item.UseMessage);
        }

        public void EndCombatAction()
        {
            /*
            this.textboxControl.Textbox.Dialog(this.state.gameActionInfo.postActionDialog, 0, () =>
            {

            });
            UIControlsManager.Instance.TakeControl(this.state.textboxControl);
            this.state = null;
            */
        }

        public void TriggerAnimationSwap()
        {
            this.animator.SetTrigger(BF2D.Game.Strings.Animation.Swap);
        }

        public abstract void TriggerGameAction();

        public void EnqueueStatsAction(CharacterStatsAction statsAction)
        {
            this.incomingStatsActions.Enqueue(statsAction);
        }
        #endregion

        #region Private Methods
        protected void TriggerGameAction(CharacterStats source)
        {
            /*
            if (this.state.gameAction is null)
                return;
            this.state.gameActionInfo = this.state.gameAction.Run(source, this.state.targets);
            */
        }

        private void SendUseMessage(string subjectName, List<string> useMessage)
        {
            /*
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

            this.textboxControl.Textbox.Dialog(dialog, 0, TriggerAnimationSwap);

            UIControlsManager.Instance.TakeControl(this.textboxControl);
            */
        }
        #endregion
    }
}
