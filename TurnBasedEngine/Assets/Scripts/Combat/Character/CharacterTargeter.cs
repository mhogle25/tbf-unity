using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BF2D.Game.Actions;
using System;
using BF2D.Enums;
using BF2D.UI;
using BF2D.Game;

namespace BF2D.Combat
{
    public class CharacterTargeter : MonoBehaviour
    {
        [SerializeField] private UIOptionsControl textGridControl = null;
        [SerializeField] private UIOptionsControl iconGridControl = null;

        [SerializeField] private DialogTextbox textbox = null;

        private readonly CharacterTargetCollection<Action<CharacterStatsAction>> targeterSetupActionCollection = new CharacterTargetCollection<Action<CharacterStatsAction>>();

        private readonly Queue<CharacterStatsAction> statsActionQueue = new Queue<CharacterStatsAction>();

        private Action state = null;
        private Action callback = null;

        private void Awake()
        {
            this.targeterSetupActionCollection[CharacterTarget.Self] = (statsAction) =>
            {
                statsAction.AddTarget(CombatManager.Instance.CurrentCharacter);
                this.state = StateEnd;
            };

            this.targeterSetupActionCollection[CharacterTarget.Player] = (statsAction) =>
            {

            };

            this.targeterSetupActionCollection[CharacterTarget.AllPlayers] = (statsAction) =>
            {

            };

            this.targeterSetupActionCollection[CharacterTarget.Enemy] = (statsAction) =>
            {

            };

            this.targeterSetupActionCollection[CharacterTarget.AllEnemies] = (statsAction) =>
            {

            };

            this.targeterSetupActionCollection[CharacterTarget.Any] = (statsAction) =>
            {

            };

            this.targeterSetupActionCollection[CharacterTarget.All] = (statsAction) =>
            {

            };
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
            this.state = StateCheckQueue;
            this.callback = callback;
        }

        #region STATES
        private void StateCheckQueue()
        {
            if (this.statsActionQueue.Count < 1)
                return;

            CharacterStatsAction statsAction = this.statsActionQueue.Dequeue();

            SetupCharacterStatsAction(statsAction);
            this.state = null;
        }

        private void StateEnd()
        {
            if (this.statsActionQueue.Count > 0)
                this.state = StateCheckQueue;

            this.callback?.Invoke();
            this.state = null;
        }
        #endregion
        
        private void SetupCharacterStatsAction(CharacterStatsAction statsAction)
        {
            this.textbox.UtilityInitialize();

            this.textbox.Message
            (
                $"Who do you want to {statsAction.Description}?",
                () =>
                {
                    this.targeterSetupActionCollection[statsAction.Target](statsAction);
                }
            );
        }
    }
}
