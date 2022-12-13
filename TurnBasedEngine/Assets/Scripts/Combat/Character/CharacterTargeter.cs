using System.Collections.Generic;
using UnityEngine;
using BF2D.Game.Actions;
using System;
using BF2D.Enums;
using BF2D.UI;

namespace BF2D.Combat
{
    public class CharacterTargeter : MonoBehaviour
    {
        [SerializeField] private OptionsGridControl playerPlatforms = null;
        [SerializeField] private OptionsGridControl enemyPlatforms = null;
        [SerializeField] private OptionsGridControl anyPlatforms = null;
        [SerializeField] private DialogTextboxControl textboxControl = null;

        private readonly CharacterTargetCollection<Action<CharacterStatsAction>> targeterSetupActionCollection = new();

        private readonly Queue<CharacterStatsAction> statsActionQueue = new();
        private readonly Queue<CombatAction> combatActions = new();
        private Action<Queue<CombatAction>> callback = null;

        private Action state = null;

        private void Awake()
        {
            this.targeterSetupActionCollection[CharacterTarget.Self] = (statsAction) =>
            {
                this.combatActions.Enqueue(new CombatAction
                {
                    StatsAction = statsAction,
                    Targets = new List<CharacterCombat> { CombatManager.Instance.CurrentCharacter }
                });
                Continue();
            };

            this.targeterSetupActionCollection[CharacterTarget.Player] = (statsAction) =>
            {
                Continue();
            };

            this.targeterSetupActionCollection[CharacterTarget.AllPlayers] = (statsAction) =>
            {
                this.combatActions.Enqueue(new CombatAction
                {
                    StatsAction = statsAction,
                    Targets = CombatManager.Instance.Players
                });
                Continue();
            };

            this.targeterSetupActionCollection[CharacterTarget.Enemy] = (statsAction) =>
            {
                Continue();
            };

            this.targeterSetupActionCollection[CharacterTarget.AllEnemies] = (statsAction) =>
            {
                this.combatActions.Enqueue(new CombatAction
                {
                    StatsAction = statsAction,
                    Targets = CombatManager.Instance.Enemies
                });
                Continue();
            };

            this.targeterSetupActionCollection[CharacterTarget.Any] = (statsAction) =>
            {
                Continue();
            };

            this.targeterSetupActionCollection[CharacterTarget.AllOfAny] = (statsAction) =>
            {
                Continue();
            };

            this.targeterSetupActionCollection[CharacterTarget.All] = (statsAction) =>
            {
                this.combatActions.Enqueue(new CombatAction
                {
                    StatsAction = statsAction,
                    Targets = CombatManager.Instance.Characters
                });
                Continue();
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

        public void Run(Action<Queue<CombatAction>> callback)
        {
            if (this.statsActionQueue.Count > 0)
            {
                Continue();
            } 
            else
            {
                Debug.LogError("[CharacterTargeter:Run] Tried to run but the StatsAction queue was empty.");
            }

            this.callback = callback;
        }

        private void Continue()
        {
            if (this.statsActionQueue.Count > 0)
            {
                ActionInit(this.statsActionQueue.Dequeue());
            }
            else
            {
                this.callback?.Invoke(this.combatActions);
            }
        }

        private void ActionInit(CharacterStatsAction statsAction)
        {
            this.targeterSetupActionCollection[statsAction.Target](statsAction);
        }
    }
}
