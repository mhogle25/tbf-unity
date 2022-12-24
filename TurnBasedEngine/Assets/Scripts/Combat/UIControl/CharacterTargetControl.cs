using System.Collections.Generic;
using UnityEngine;
using BF2D.Game.Actions;
using System;
using BF2D.Enums;
using BF2D.UI;
using BF2D.Combat.Actions;

namespace BF2D.Combat
{
    public class CharacterTargetControl : OptionsGridControl
    {
        [SerializeField] private OptionsGrid playerPlatforms = null;
        [SerializeField] private OptionsGrid enemyPlatforms = null;
        [SerializeField] private OptionsGrid anyPlatforms = null;
        [SerializeField] private OptionsGrid allOfAnyMenu = null;
        [SerializeField] private DialogTextbox textbox = null;

        private readonly CharacterTargetCollection<Action<CharacterStatsAction>> targeterSetupActionCollection = new();
        private readonly Queue<CharacterStatsAction> stagedStatsActions = null;
        private CharacterStatsAction stagedStatsAction = null;

        private readonly Queue<TargetedStatsAction> combatActions = new();

        public override void ControlInitialize()
        {
            List<CharacterStatsAction> characterStatsActions = CombatManager.Instance.CurrentCharacter.CurrentCombatAction.GetStatsActions();
            foreach (CharacterStatsAction statsAction in characterStatsActions)
            {
                this.stagedStatsActions.Enqueue(statsAction);
            }
            Continue();
        }

        public override void ControlFinalize() 
        { 

        }

        protected override void Awake()
        {
            base.Awake();
            
            this.targeterSetupActionCollection[CharacterTarget.Self] = (statsAction) =>
            {
                this.combatActions.Enqueue(new TargetedStatsAction
                {
                    StatsAction = statsAction,
                    Targets = new List<CharacterCombat> { CombatManager.Instance.CurrentCharacter }
                });
                Continue();
            };

            this.targeterSetupActionCollection[CharacterTarget.Player] = (statsAction) =>
            {
                SetupDialog(this.playerPlatforms);
            };

            this.targeterSetupActionCollection[CharacterTarget.AllPlayers] = (statsAction) =>
            {
                this.combatActions.Enqueue(new TargetedStatsAction
                {
                    StatsAction = statsAction,
                    Targets = CombatManager.Instance.Players
                });
                Continue();
            };

            this.targeterSetupActionCollection[CharacterTarget.Enemy] = (statsAction) =>
            {
                SetupDialog(this.enemyPlatforms);
            };

            this.targeterSetupActionCollection[CharacterTarget.AllEnemies] = (statsAction) =>
            {
                this.combatActions.Enqueue(new TargetedStatsAction
                {
                    StatsAction = statsAction,
                    Targets = CombatManager.Instance.Enemies
                });
                Continue();
            };

            this.targeterSetupActionCollection[CharacterTarget.Any] = (statsAction) =>
            {
                SetupDialog(this.anyPlatforms);
            };

            this.targeterSetupActionCollection[CharacterTarget.AllOfAny] = (statsAction) =>
            {
                SetupDialog(this.allOfAnyMenu);
            };

            this.targeterSetupActionCollection[CharacterTarget.All] = (statsAction) =>
            {
                this.combatActions.Enqueue(new TargetedStatsAction
                {
                    StatsAction = statsAction,
                    Targets = CombatManager.Instance.Characters
                });
                Continue();
            };
        }

        public void SetTargets(List<CharacterCombat> targets)
        {
            this.combatActions.Enqueue(new TargetedStatsAction
            {
                StatsAction = this.stagedStatsAction,
                Targets = targets
            });

            this.controlledOptionsGrid.UtilityFinalize();
            Continue();
        }

        private void Continue()
        {
            if (this.stagedStatsActions.Count < 1) 
            {
                CombatManager.Instance.CurrentCharacter.CurrentCombatAction.SetTargetedStatsActions(this.combatActions);
                CombatManager.Instance.RunCombat();
                return;
            }

            this.stagedStatsAction = this.stagedStatsActions.Dequeue();
            this.targeterSetupActionCollection[this.stagedStatsAction.Target](this.stagedStatsAction);
        }

        private void SetupDialog(OptionsGrid followUp)
        {
            textbox.Message($"Who would you like to {this.stagedStatsAction.Description}?", () =>
            {
                this.textbox.UtilityFinalize();
                this.controlledOptionsGrid = followUp;
                this.controlledOptionsGrid.UtilityInitialize();
            });
        }
    }
}
