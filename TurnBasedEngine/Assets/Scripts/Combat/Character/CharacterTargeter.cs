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
        [SerializeField] private UIOptionsControl enemyOptionsControl = null;
        [SerializeField] private UIOptionsControl playerOptionsControl = null;
        [SerializeField] private UIOptionsControl anyOptionsControl = null;

        [SerializeField] private DialogTextboxControl textboxControl = null;

        private readonly CharacterTargetCollection<Action<CharacterStatsAction>> targeterSetupActionCollection = new();

        private readonly Queue<CharacterStatsAction> statsActionQueue = new();

        private Action state = null;
        private Action callback = null;

        private void Awake()
        {
            this.targeterSetupActionCollection[CharacterTarget.Self] = (statsAction) =>
            {

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

            this.targeterSetupActionCollection[CharacterTarget.AllOfAny] = (statsAction) =>
            {

            };

            this.targeterSetupActionCollection[CharacterTarget.All] = (statsAction) =>
            {

            };
        }

        private void Update()
        {
            this.state?.Invoke();
        }

        public void Enqueue(CharacterStatsAction statsAction)
        {
            this.statsActionQueue.Enqueue(statsAction);
        }

        public void Run(Action callback)
        {
            this.callback = callback;
            this.state = StateStandby;
        }

        private void StateStandby()
        {
            if (this.statsActionQueue.Count > 0)
            {
                ActionInit(this.statsActionQueue.Dequeue());
                this.state = null;
            }
        }
        
        private void ActionInit(CharacterStatsAction statsAction)
        {
            this.targeterSetupActionCollection[statsAction.Target](statsAction);
        }
    }
}
