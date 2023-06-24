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

        private readonly Queue<TargetedCharacterStatsActionSlot> stagedTargetedGemSlots = new();
        private TargetedCharacterStatsActionSlot stagedTargetedGemSlot = null;

        public override void ControlInitialize()
        {
            this.stagedTargetedGemSlots.Clear();
            this.stagedTargetedGemSlot = null;
            foreach (TargetedCharacterStatsActionSlot targetedGemSlot in CombatManager.Instance.CurrentCharacter.CurrentCombatAction.GetTargetedGemSlots())
                this.stagedTargetedGemSlots.Enqueue(targetedGemSlot);

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
                this.orphanedTextbox.Continue();
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
                    grid.Add(tile);
            }
        }

        public void SetSingleTarget(CharacterCombat target)
        {
            this.stagedTargetedGemSlot.TargetInfo.CombatTargets = new CharacterCombat[] { target };
            Continue();
        }

        private void TargeterSetup(CharacterTarget target)
        {
            switch (target)
            {
                case CharacterTarget.Self:
                    this.stagedTargetedGemSlot.TargetInfo.CombatTargets = new CharacterCombat[] { CombatManager.Instance.CurrentCharacter };
                    Continue();
                    return;
                case CharacterTarget.Ally:
                    SetupDialog(this.playerPlatforms);
                    return;
                case CharacterTarget.AllAllies:
                    this.stagedTargetedGemSlot.TargetInfo.CombatTargets = CombatManager.Instance.Players;
                    Continue();
                    return;
                case CharacterTarget.Opponent:
                    SetupDialog(this.enemyPlatforms);
                    return;
                case CharacterTarget.AllOpponents:
                    this.stagedTargetedGemSlot.TargetInfo.CombatTargets = CombatManager.Instance.Enemies;
                    Continue();
                    return;
                case CharacterTarget.Any:
                    SetupDialog(this.anyPlatforms);
                    return;
                case CharacterTarget.AllOfAny:
                    AllOfAnyEvent();
                    return;
                case CharacterTarget.All:
                    this.stagedTargetedGemSlot.TargetInfo.CombatTargets = CombatManager.Instance.Characters;
                    Continue();
                    return;
                case CharacterTarget.Random:
                    this.stagedTargetedGemSlot.TargetInfo.CombatTargets = new CharacterCombat[] { CombatManager.Instance.RandomCharacter() };
                    Continue();
                    return;
                case CharacterTarget.RandomAlly:
                    this.stagedTargetedGemSlot.TargetInfo.CombatTargets = new CharacterCombat[] { CombatManager.Instance.RandomPlayer() };
                    Continue();
                    return;
                case CharacterTarget.RandomOpponent:
                    this.stagedTargetedGemSlot.TargetInfo.CombatTargets = new CharacterCombat[] { CombatManager.Instance.RandomEnemy() };
                    Continue();
                    return;
                default:
                    Debug.LogError("[CharacterTargeterControl:TargeterSetup] The provided value for a character target was invalid");
                    return;
            }
        }

        private void Continue()
        {
            if (this.controlled)
            {
                this.controlled.SetCursorAtPosition(this.controlled.CursorPosition, false);
                this.controlled.UtilityFinalize();
            }

            if (this.stagedTargetedGemSlots.Count < 1) 
            {
                this.orphanedTextbox.UtilityFinalize();
                this.orphanedTextbox.View.gameObject.SetActive(false);

                CombatManager.Instance.RunCombatEvents();
                return;
            }

            this.stagedTargetedGemSlot = this.stagedTargetedGemSlots.Dequeue();
            TargeterSetup(this.stagedTargetedGemSlot.Target);
        }

        private void SetupDialog(OptionsGrid followUp)
        {
            this.orphanedTextbox.messageInterrupt = true;
            this.orphanedTextbox.autoPass = true;
            this.orphanedTextbox.Dialog("di_targeter", 0, () =>
            {
                this.orphanedTextbox.UtilityFinalize();
                if (this.controlled && this.controlled.Interactable)
                    this.controlled.UtilityFinalize();

                this.controlled = followUp;
                this.controlled.UtilityInitialize();
                this.controlled.SetCursorToFirst();
                this.orphanedTextbox.messageInterrupt = default;
                this.orphanedTextbox.autoPass = default;
            },
            new string[]
            {
                this.stagedTargetedGemSlot.Description
            });
            this.orphanedTextbox.UtilityInitialize();
        }

        private void AllOfAnyEvent()
        {
            this.orphanedTextbox.messageInterrupt = true;
            this.orphanedTextbox.autoPass = false;
            this.orphanedTextbox.ResponseBackEventEnabled = true; 
            this.orphanedTextbox.ResponseConfirmEvent.AddListener((json) =>
            {
                AlignmentFlag flag = Utilities.JSON.DeserializeString<AlignmentFlag>(json);
                this.stagedTargetedGemSlot.TargetInfo.CombatTargets = flag.players ? CombatManager.Instance.Players : CombatManager.Instance.Enemies;
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
            new string[]
            {
                this.stagedTargetedGemSlot.Description
            });
            this.orphanedTextbox.UtilityInitialize();
        }

        private void EndAllOfAnyEvent()
        {
            this.orphanedTextbox.ResponseConfirmEvent.RemoveAllListeners();
            this.orphanedTextbox.ResponseBackEvent.RemoveAllListeners();
            this.orphanedTextbox.autoPass = default;
            this.orphanedTextbox.messageInterrupt = default;
            this.orphanedTextbox.ResponseBackEventEnabled = default;
        }
    }
}
