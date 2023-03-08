using System.Collections.Generic;
using UnityEngine;
using BF2D.Game.Actions;
using System;
using BF2D.Enums;
using BF2D.UI;
using BF2D.Game.Combat.Actions;

namespace BF2D.Game.Combat
{
    public class CharacterTargeterControl : OptionsGridControl
    {
        public struct AlignmentFlag
        {
            public bool players;
        }

        [Header("Dialog Textbox")]
        [SerializeField] private DialogTextbox orphanedTextbox = null;
        [Header("Character Targeter")]
        [SerializeField] private OptionsGrid playerPlatforms = null;
        [SerializeField] private List<CombatGridTile> initPlayerOptions = new();
        [Space(10)]
        [SerializeField] private OptionsGrid enemyPlatforms = null;
        [SerializeField] private List<CombatGridTile> initEnemyOptions = new();
        [Space(10)]
        [SerializeField] private OptionsGrid anyPlatforms = null;
        [SerializeField] private List<CombatGridTile> initAnyOptions = new();

        private readonly Queue<TargetedCharacterStatsAction> stagedStatsActions = new();
        private TargetedCharacterStatsAction stagedStatsAction = null;

        public override void ControlInitialize()
        {
            this.stagedStatsActions.Clear();
            this.stagedStatsAction = null;
            foreach (TargetedCharacterStatsAction statsAction in CombatManager.Instance.CurrentCharacter.CurrentCombatAction.GetTargetedGems())
            {
                this.stagedStatsActions.Enqueue(statsAction);
            }
            Continue();
        }

        public override void ControlFinalize() 
        {
            this.orphanedTextbox.View.gameObject.SetActive(false);
            if (this.controlledOptionsGrid)
            {
                this.controlledOptionsGrid.SetCursorAtPosition(this.controlledOptionsGrid.CursorPosition, false);
                this.controlledOptionsGrid.UtilityFinalize();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            LoadOptionsIntoGrid(this.playerPlatforms, this.initPlayerOptions);
            LoadOptionsIntoGrid(this.enemyPlatforms, this.initEnemyOptions);
            LoadOptionsIntoGrid(this.anyPlatforms, this.initAnyOptions);
        }

        protected override void Update()
        {
            base.Update();

            if (InputManager.Instance.ConfirmPress)
            {
                this.orphanedTextbox.Continue();
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
            this.stagedStatsAction.TargetInfo.CombatTargets = targets;

            this.controlledOptionsGrid.SetCursorAtPosition(this.controlledOptionsGrid.CursorPosition, false);
            this.controlledOptionsGrid.UtilityFinalize();
            Continue();
        }

        private void TargeterSetup(CharacterTarget target)
        {
            switch (target)
            {
                case CharacterTarget.Self:
                    this.stagedStatsAction.TargetInfo.CombatTargets = new List<CharacterCombat> { CombatManager.Instance.CurrentCharacter };
                    Continue();
                    return;
                case CharacterTarget.Ally:
                    SetupDialog(this.playerPlatforms);
                    return;
                case CharacterTarget.AllAllies:
                    this.stagedStatsAction.TargetInfo.CombatTargets = CombatManager.Instance.Players;
                    Continue();
                    return;
                case CharacterTarget.Opponent:
                    SetupDialog(this.enemyPlatforms);
                    return;
                case CharacterTarget.AllOpponents:
                    this.stagedStatsAction.TargetInfo.CombatTargets = CombatManager.Instance.Enemies;
                    Continue();
                    return;
                case CharacterTarget.Any:
                    SetupDialog(this.anyPlatforms);
                    return;
                case CharacterTarget.AllOfAny:
                    AllOfAnyEvent();
                    return;
                case CharacterTarget.All:
                    this.stagedStatsAction.TargetInfo.CombatTargets = CombatManager.Instance.Characters;
                    Continue();
                    return;
                case CharacterTarget.Random:
                    this.stagedStatsAction.TargetInfo.CombatTargets = new List<CharacterCombat> { CombatManager.Instance.RandomCharacter() };
                    Continue();
                    return;
                case CharacterTarget.RandomAlly:
                    this.stagedStatsAction.TargetInfo.CombatTargets = new List<CharacterCombat> { CombatManager.Instance.RandomPlayer() };
                    Continue();
                    return;
                case CharacterTarget.RandomOpponent:
                    this.stagedStatsAction.TargetInfo.CombatTargets = new List<CharacterCombat> { CombatManager.Instance.RandomEnemy() };
                    Continue();
                    return;
                default:
                    Terminal.IO.LogError("[CharacterTargeterControl:TargeterSetup] The provided value for a character target was invalid");
                    return;
            }
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

                this.orphanedTextbox.UtilityFinalize();
                this.orphanedTextbox.View.gameObject.SetActive(false);

                CombatManager.Instance.RunCombatEvents();
                return;
            }
            this.stagedStatsAction = this.stagedStatsActions.Dequeue();
            TargeterSetup(this.stagedStatsAction.Target);
        }

        private void SetupDialog(OptionsGrid followUp)
        {
            this.orphanedTextbox.Dialog("di_targeter", 0, () =>
            {
                this.orphanedTextbox.UtilityFinalize();
                if (this.controlledOptionsGrid && this.controlledOptionsGrid.Interactable)
                {
                    this.controlledOptionsGrid.UtilityFinalize();
                }
                this.controlledOptionsGrid = followUp;
                this.controlledOptionsGrid.UtilityInitialize();
                this.controlledOptionsGrid.SetCursorAtPosition(this.controlledOptionsGrid.CursorPosition, true);
            },
            new List<string>
            {
                this.stagedStatsAction.Description
            });
            this.orphanedTextbox.UtilityInitialize();
        }

        private void AllOfAnyEvent()
        {
            this.orphanedTextbox.AutoPass = false;
            this.orphanedTextbox.ResponseBackEventEnabled = true; 
            this.orphanedTextbox.ResponseEvent.AddListener((json) =>
            {
                AlignmentFlag flag = BF2D.Utilities.TextFile.DeserializeString<AlignmentFlag>(json);
                this.stagedStatsAction.TargetInfo.CombatTargets = flag.players ? CombatManager.Instance.Players : CombatManager.Instance.Enemies;
            });
            this.orphanedTextbox.ResponseBackEvent.AddListener(() =>
            {
                EndAllOfAnyEvent();
                this.orphanedTextbox.View.gameObject.SetActive(false);
                UIControlsManager.Instance.PassControlBack();
                this.orphanedTextbox.Cancel();
            });
            this.orphanedTextbox.Dialog("di_targeter", 1, () =>
            {
                EndAllOfAnyEvent();
                Continue();
            },
            new List<string>
            {
                this.stagedStatsAction.Description
            });
            this.orphanedTextbox.UtilityInitialize();
        }

        private void EndAllOfAnyEvent()
        {
            this.orphanedTextbox.ResponseEvent.RemoveAllListeners();
            this.orphanedTextbox.ResponseBackEvent.RemoveAllListeners();
            this.orphanedTextbox.AutoPass = true;
            this.orphanedTextbox.ResponseBackEventEnabled = false;
        }
    }
}
