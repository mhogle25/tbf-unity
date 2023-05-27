using System.Collections.Generic;
using UnityEngine;
using BF2D.Game.Actions;
using BF2D.Enums;
using BF2D.UI;

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

        private readonly Queue<TargetedCharacterStatsAction> stagedTargetedGems = new();
        private TargetedCharacterStatsAction stagedTargetedGem = null;

        public override void ControlInitialize()
        {
            this.stagedTargetedGems.Clear();
            this.stagedTargetedGem = null;
            foreach (TargetedCharacterStatsAction targetedGem in CombatManager.Instance.CurrentCharacter.CurrentCombatAction.GetTargetedGems())
            {
                this.stagedTargetedGems.Enqueue(targetedGem);
            }
            Continue();
        }

        public override void ControlFinalize() 
        {
            this.orphanedTextbox.View.gameObject.SetActive(false);
            if (this.controlled)
            {
                this.controlled.SetCursorAtPosition(this.controlled.CursorPosition, false);
                this.controlled.UtilityFinalize();
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

        private void LoadOptionsIntoGrid(OptionsGrid grid, List<CombatGridTile> initGridOptions)
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

        public void SetSingleTarget(CharacterCombat target)
        {
            this.stagedTargetedGem.TargetInfo.CombatTargets = new CharacterCombat[] { target };

            this.controlled.SetCursorAtPosition(this.controlled.CursorPosition, false);
            this.controlled.UtilityFinalize();
            Continue();
        }

        private void TargeterSetup(CharacterTarget target)
        {
            switch (target)
            {
                case CharacterTarget.Self:
                    this.stagedTargetedGem.TargetInfo.CombatTargets = new CharacterCombat[] { CombatManager.Instance.CurrentCharacter };
                    Continue();
                    return;
                case CharacterTarget.Ally:
                    SetupDialog(this.playerPlatforms);
                    return;
                case CharacterTarget.AllAllies:
                    this.stagedTargetedGem.TargetInfo.CombatTargets = CombatManager.Instance.Players;
                    Continue();
                    return;
                case CharacterTarget.Opponent:
                    SetupDialog(this.enemyPlatforms);
                    return;
                case CharacterTarget.AllOpponents:
                    this.stagedTargetedGem.TargetInfo.CombatTargets = CombatManager.Instance.Enemies;
                    Continue();
                    return;
                case CharacterTarget.Any:
                    SetupDialog(this.anyPlatforms);
                    return;
                case CharacterTarget.AllOfAny:
                    AllOfAnyEvent();
                    return;
                case CharacterTarget.All:
                    this.stagedTargetedGem.TargetInfo.CombatTargets = CombatManager.Instance.Characters;
                    Continue();
                    return;
                case CharacterTarget.Random:
                    this.stagedTargetedGem.TargetInfo.CombatTargets = new CharacterCombat[] { CombatManager.Instance.RandomCharacter() };
                    Continue();
                    return;
                case CharacterTarget.RandomAlly:
                    this.stagedTargetedGem.TargetInfo.CombatTargets = new CharacterCombat[] { CombatManager.Instance.RandomPlayer() };
                    Continue();
                    return;
                case CharacterTarget.RandomOpponent:
                    this.stagedTargetedGem.TargetInfo.CombatTargets = new CharacterCombat[] { CombatManager.Instance.RandomEnemy() };
                    Continue();
                    return;
                default:
                    Debug.LogError("[CharacterTargeterControl:TargeterSetup] The provided value for a character target was invalid");
                    return;
            }
        }

        private void Continue()
        {
            if (this.stagedTargetedGems.Count < 1) 
            {
                if (this.controlled)
                {
                    this.controlled.SetCursorAtPosition(this.controlled.CursorPosition, false);
                    this.controlled.UtilityFinalize();
                }

                this.orphanedTextbox.UtilityFinalize();
                this.orphanedTextbox.View.gameObject.SetActive(false);

                CombatManager.Instance.RunCombatEvents();
                return;
            }
            this.stagedTargetedGem = this.stagedTargetedGems.Dequeue();
            TargeterSetup(this.stagedTargetedGem.Target);
        }

        private void SetupDialog(OptionsGrid followUp)
        {
            this.orphanedTextbox.MessageInterrupt = true;
            this.orphanedTextbox.Dialog("di_targeter", false, 0, () =>
            {
                this.orphanedTextbox.UtilityFinalize();
                if (this.controlled && this.controlled.Interactable)
                {
                    this.controlled.UtilityFinalize();
                }
                this.controlled = followUp;
                this.controlled.UtilityInitialize();
                this.controlled.SetCursorAtPosition(this.controlled.CursorPosition, true);
                this.orphanedTextbox.MessageInterrupt = false;
            },
            new string[]
            {
                this.stagedTargetedGem.Description
            });
            this.orphanedTextbox.UtilityInitialize();
        }

        private void AllOfAnyEvent()
        {
            this.orphanedTextbox.MessageInterrupt = true;
            this.orphanedTextbox.AutoPass = false;
            this.orphanedTextbox.ResponseBackEventEnabled = true; 
            this.orphanedTextbox.ResponseConfirmEvent.AddListener((json) =>
            {
                AlignmentFlag flag = BF2D.Utilities.JSON.DeserializeString<AlignmentFlag>(json);
                this.stagedTargetedGem.TargetInfo.CombatTargets = flag.players ? CombatManager.Instance.Players : CombatManager.Instance.Enemies;
            });
            this.orphanedTextbox.ResponseBackEvent.AddListener(() =>
            {
                EndAllOfAnyEvent();
                this.orphanedTextbox.View.gameObject.SetActive(false);
                UIControlsManager.Instance.PassControlBack();
                this.orphanedTextbox.Cancel();
            });
            this.orphanedTextbox.Dialog("di_targeter", true, 1, () =>
            {
                EndAllOfAnyEvent();
                Continue();
            },
            new string[]
            {
                this.stagedTargetedGem.Description
            });
            this.orphanedTextbox.UtilityInitialize();
        }

        private void EndAllOfAnyEvent()
        {
            this.orphanedTextbox.ResponseConfirmEvent.RemoveAllListeners();
            this.orphanedTextbox.ResponseBackEvent.RemoveAllListeners();
            this.orphanedTextbox.AutoPass = true;
            this.orphanedTextbox.MessageInterrupt = false;
            this.orphanedTextbox.ResponseBackEventEnabled = false;
        }
    }
}
