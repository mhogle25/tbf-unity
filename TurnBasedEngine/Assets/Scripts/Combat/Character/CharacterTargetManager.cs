using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BF2D.Actions;
using System;
using BF2D.Enums;
using BF2D.UI;
using BF2D.Game;

namespace BF2D.Combat
{
    public class CharacterTargetManager : MonoBehaviour
    {
        [SerializeField] private UIOptionsControl textGridControl = null;
        [SerializeField] private UIOptionsControl iconGridControl = null;

        [SerializeField] private DialogTextbox textbox = null;

        private CharacterTargetCollection<Action> targeterSetupActionCollection = new CharacterTargetCollection<Action>();

        private Queue<CharacterStatsAction> statsActionQueue = new Queue<CharacterStatsAction>();

        private Action state = null;
        private Action callback = null;

        private void Awake()
        {
            this.targeterSetupActionCollection[CharacterTarget.Self] = null;
            this.targeterSetupActionCollection[CharacterTarget.Player] = null;
            this.targeterSetupActionCollection[CharacterTarget.AllPlayers] = null;
            this.targeterSetupActionCollection[CharacterTarget.Enemy] = null;
            this.targeterSetupActionCollection[CharacterTarget.AllEnemies] = null;
            this.targeterSetupActionCollection[CharacterTarget.Any] = null;
            this.targeterSetupActionCollection[CharacterTarget.All] = null;
        }

        private void Update()
        {
            state?.Invoke();
        }

        public void Enqueue(CharacterStatsAction statsAction)
        {
            this.statsActionQueue.Enqueue(statsAction);
        }

        public void Execute(Action callback)
        {
            this.state = CheckQueue;
            this.callback = callback;
        }

        #region STATES
        private void CheckQueue()
        {
            CharacterStatsAction statsAction = this.statsActionQueue.Dequeue();
            if (statsAction is null)
                return;
            SetupCharacterStatsAction(statsAction);
            this.state = null;
        }
        #endregion
        
        private void SetupCharacterStatsAction(CharacterStatsAction statsAction)
        {
            TextboxInteractable(true);

            this.textbox.Message
            (
                $"Who do you want to {statsAction.Description}?",
                () =>
                {
                    this.targeterSetupActionCollection[statsAction.Target]();
                }
            );
        }

        private void TextboxInteractable(bool value)
        {
            this.textbox.View.gameObject.SetActive(value);
            this.textbox.Interactable = value;
        }
    }
}
