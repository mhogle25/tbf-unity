using System.Collections.Generic;
using UnityEngine;
using BF2D.Game.Actions;
using System;
using BF2D.Enums;
using BF2D.UI;
using BF2D.Combat.Actions;

namespace BF2D.Combat
{
    public class CharacterTargeterControl : OptionsGridControl
    {
        [Header("Character Targeter")]
        [SerializeField] private OptionsGrid playerPlatforms = null;
        [SerializeField] private List<CombatGridTile> initPlayerOptions = new();
        [SerializeField] private OptionsGrid enemyPlatforms = null;
        [SerializeField] private List<CombatGridTile> initEnemyOptions = new();
        [SerializeField] private OptionsGrid anyPlatforms = null;
        [SerializeField] private List<CombatGridTile> initAnyOptions = new();
        [Header("Misc")]
        [SerializeField] private DialogTextbox textbox = null;

        private readonly CharacterTargetCollection<Action> targeterSetupActionCollection = new();
        private readonly Queue<CharacterStatsAction> stagedStatsActions = new();
        private CharacterStatsAction stagedStatsAction = null;

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
            this.textbox.View.gameObject.SetActive(false);
            this.controlledOptionsGrid.SetCursorAtPosition(this.controlledOptionsGrid.CursorPosition, false);
            this.controlledOptionsGrid.UtilityFinalize();
        }

        protected override void Awake()
        {
            base.Awake();
            SetupTargeterActionsCollection();
            LoadOptionsIntoGrid(this.playerPlatforms, this.initPlayerOptions);
            LoadOptionsIntoGrid(this.enemyPlatforms, this.initEnemyOptions);
            LoadOptionsIntoGrid(this.anyPlatforms, this.initAnyOptions);
        }

        protected override void Update()
        {
            base.Update();

            if (InputManager.ConfirmPress)
            {
                this.textbox.Continue();
            }
        }

        protected void LoadOptionsIntoGrid(OptionsGrid grid, List<CombatGridTile> initGridOptions)
        {
            if (grid.Width > 0 && grid.Height > 0)
            {
                //Create the element data structure
                grid.Setup(grid.Width, grid.Height);
            }

            if (initGridOptions != null && initGridOptions.Count > 0)
            {
                foreach (CombatGridTile tile in initGridOptions)
                {
                    grid.Add(tile);
                }
            }
        }

        public void SetTargets(List<CharacterCombat> targets)
        {
            CombatManager.Instance.CurrentCharacter.CurrentCombatAction.EnqueueTargetedStatsActions(new TargetedStatsAction
            {
                StatsAction = this.stagedStatsAction,
                Targets = targets
            });

            this.controlledOptionsGrid.UtilityFinalize();
            Continue();
        }

        private void SetupTargeterActionsCollection()
        {
            this.targeterSetupActionCollection[CharacterTarget.Self] = () =>
            {
                CombatManager.Instance.CurrentCharacter.CurrentCombatAction.EnqueueTargetedStatsActions(new TargetedStatsAction
                {
                    StatsAction = this.stagedStatsAction,
                    Targets = new List<CharacterCombat> { CombatManager.Instance.CurrentCharacter }
                });
                Continue();
            };

            this.targeterSetupActionCollection[CharacterTarget.Player] = () =>
            {
                SetupDialog(this.playerPlatforms);
            };

            this.targeterSetupActionCollection[CharacterTarget.AllPlayers] = () =>
            {
                CombatManager.Instance.CurrentCharacter.CurrentCombatAction.EnqueueTargetedStatsActions(new TargetedStatsAction
                {
                    StatsAction = this.stagedStatsAction,
                    Targets = CombatManager.Instance.Players
                });
                Continue();
            };

            this.targeterSetupActionCollection[CharacterTarget.Enemy] = () =>
            {
                SetupDialog(this.enemyPlatforms);
            };

            this.targeterSetupActionCollection[CharacterTarget.AllEnemies] = () =>
            {
                CombatManager.Instance.CurrentCharacter.CurrentCombatAction.EnqueueTargetedStatsActions(new TargetedStatsAction
                {
                    StatsAction = this.stagedStatsAction,
                    Targets = CombatManager.Instance.Enemies
                });
                Continue();
            };

            this.targeterSetupActionCollection[CharacterTarget.Any] = () =>
            {
                SetupDialog(this.anyPlatforms);
            };

            this.targeterSetupActionCollection[CharacterTarget.AllOfAny] = () =>
            {
                //TODO
            };

            this.targeterSetupActionCollection[CharacterTarget.All] = () =>
            {
                CombatManager.Instance.CurrentCharacter.CurrentCombatAction.EnqueueTargetedStatsActions(new TargetedStatsAction
                {
                    StatsAction = this.stagedStatsAction,
                    Targets = CombatManager.Instance.Characters
                });
                Continue();
            };
        }

        private void Continue()
        {
            if (this.stagedStatsActions.Count < 1) 
            {
                if (this.controlledOptionsGrid)
                {
                    this.controlledOptionsGrid.SetCursorAtPosition(this.controlledOptionsGrid.CursorPosition, false);
                    this.controlledOptionsGrid.UtilityFinalize();
                }
                if (this.textbox)
                {
                    this.textbox.UtilityFinalize();
                    this.textbox.View.gameObject.SetActive(false);
                }
                CombatManager.Instance.RunCombat();
                return;
            }
            this.stagedStatsAction = this.stagedStatsActions.Dequeue();
            this.targeterSetupActionCollection[this.stagedStatsAction.Target]();
        }

        private void SetupDialog(OptionsGrid followUp)
        {
            this.textbox.Message($"Who would you like to {this.stagedStatsAction.Description}?", () =>
            {
                this.textbox.UtilityFinalize();
                if (this.controlledOptionsGrid && this.controlledOptionsGrid.Interactable)
                {
                    this.controlledOptionsGrid.UtilityFinalize();
                }
                this.controlledOptionsGrid = followUp;
                this.controlledOptionsGrid.UtilityInitialize();
                this.controlledOptionsGrid.SetCursorAtPosition(this.controlledOptionsGrid.CursorPosition, true);
            });
            this.textbox.UtilityInitialize();
        }
    }
}
