using System;
using System.Collections.Generic;
using UnityEngine;
using BF2D.Game.Actions;
using BF2D.Game.Enums;
using BF2D.UI;

namespace BF2D.Game.Combat
{
    public class ItemsCharacterTargeter : OptionsGridControl
    {
        [Serializable]
        public struct AlignmentFlag
        {
            public bool players;
        }

        [Header("Items Character Targeter")]
        [SerializeField] private DialogTextbox orphanedTextbox = null;
        [SerializeField] private CharacterTargeterData data = null;
        [SerializeField] private string dialogKey = "di_items_character_targeter";

        private readonly Queue<TargetedCharacterActionSlot> stagedTargetedGemSlots = new();
        private TargetedCharacterActionSlot stagedTargetedGemSlot = null;

        public override void ControlInitialize()
        {
            this.stagedTargetedGemSlots.Clear();
            this.stagedTargetedGemSlot = null;
            foreach (TargetedCharacterActionSlot targetedGemSlot in CombatCtx.One.CurrentCharacter.CurrentCombatAction.GetTargetedGameAction().TargetedGemSlots)
                this.stagedTargetedGemSlots.Enqueue(targetedGemSlot);

            Continue();
        }

        public override void ControlFinalize() 
        {
            this.orphanedTextbox.View.gameObject.SetActive(false);
            if (this.Controlled)
            {
                this.Controlled.CurrentOption.SetCursor(false);
                this.Controlled.UtilityFinalize();
            }
        }

        protected override void Update()
        {
            base.Update();

            if (InputCtx.One.ConfirmPress)
                this.orphanedTextbox.Continue();
        }

        public void SetSingleTarget(CharacterCombat target)
        {
            this.stagedTargetedGemSlot.TargetInfo.CombatTargets = new CharacterCombat[] { target };
            Continue();
        }

        private void TargeterSetup(CharacterTarget target)
        {
            CharacterAlignment alignment = CombatCtx.One.CurrentCharacter.Alignment;
            switch (target)
            {
                case CharacterTarget.Self:
                    this.stagedTargetedGemSlot.TargetInfo.CombatTargets = new CharacterCombat[] { CombatCtx.One.CurrentCharacter };
                    Continue();
                    return;
                case CharacterTarget.Ally:
                    SetupDialog(this.data.PlayerPlatforms);
                    return;
                case CharacterTarget.AllAllies:
                    this.stagedTargetedGemSlot.TargetInfo.CombatTargets = CombatCtx.One.GetAllies(alignment);
                    Continue();
                    return;
                case CharacterTarget.Opponent:
                    SetupDialog(this.data.EnemyPlatforms);
                    return;
                case CharacterTarget.AllOpponents:
                    this.stagedTargetedGemSlot.TargetInfo.CombatTargets = CombatCtx.One.GetOpponents(alignment);
                    Continue();
                    return;
                case CharacterTarget.Any:
                    SetupDialog(this.data.AnyPlatforms);
                    return;
                case CharacterTarget.AllOfAny:
                    AllOfAnyEvent();
                    return;
                case CharacterTarget.All:
                    this.stagedTargetedGemSlot.TargetInfo.CombatTargets = CombatCtx.One.Characters;
                    Continue();
                    return;
                case CharacterTarget.Random:
                    this.stagedTargetedGemSlot.TargetInfo.CombatTargets = new CharacterCombat[] { CombatCtx.One.RandomCharacter() };
                    Continue();
                    return;
                case CharacterTarget.RandomAlly:
                    this.stagedTargetedGemSlot.TargetInfo.CombatTargets = new CharacterCombat[] { CombatCtx.One.RandomAlly(alignment) };
                    Continue();
                    return;
                case CharacterTarget.RandomOpponent:
                    this.stagedTargetedGemSlot.TargetInfo.CombatTargets = new CharacterCombat[] { CombatCtx.One.RandomOpponent(alignment) };
                    Continue();
                    return;
                default:
                    Debug.LogError("[CharacterTargeterControl:TargeterSetup] The provided value for a character target was invalid");
                    return;
            }
        }

        private void Continue()
        {
            if (this.Controlled)
            {
                this.Controlled.CurrentOption.SetCursor(false);
                this.Controlled.UtilityFinalize();
            }

            if (this.stagedTargetedGemSlots.Count < 1) 
            {
                this.orphanedTextbox.UtilityFinalize();
                this.orphanedTextbox.View.gameObject.SetActive(false);

                CombatCtx.One.RunCombatEvents();
                return;
            }

            this.stagedTargetedGemSlot = this.stagedTargetedGemSlots.Dequeue();
            TargeterSetup(this.stagedTargetedGemSlot.Target);
        }

        private void SetupDialog(OptionsGrid followUp)
        {
            this.orphanedTextbox.messageInterrupt = true;
            this.orphanedTextbox.autoPass = true;

            this.orphanedTextbox.Dialog(this.dialogKey, 0, () =>
            {
                this.orphanedTextbox.UtilityFinalize();
                if (this.Controlled && this.Controlled.Interactable)
                    this.Controlled.UtilityFinalize();

                this.Controlled = followUp;
                this.Controlled.UtilityInitialize();
                this.orphanedTextbox.messageInterrupt = default;
                this.orphanedTextbox.autoPass = default;
            },
            this.stagedTargetedGemSlot.Description);

            this.orphanedTextbox.UtilityInitialize();
        }

        private void AllOfAnyEvent()
        {
            this.orphanedTextbox.messageInterrupt = true;
            this.orphanedTextbox.autoPass = false;
            this.orphanedTextbox.ResponseBackEventEnabled = true;

            this.orphanedTextbox.ResponseConfirmEvent.AddListener((json) =>
            {
                AlignmentFlag flag = Utilities.JSON.DeserializeJson<AlignmentFlag>(json);
                this.stagedTargetedGemSlot.TargetInfo.CombatTargets = flag.players ? CombatCtx.One.Players : CombatCtx.One.Enemies;
            });

            this.orphanedTextbox.ResponseBackEvent.AddListener(() =>
            {
                EndAllOfAnyEvent();
                this.orphanedTextbox.View.gameObject.SetActive(false);
                UICtx.One.PassControlBack();
                this.orphanedTextbox.Cancel();
            });

            this.orphanedTextbox.Dialog(this.dialogKey, 1, () =>
            {
                EndAllOfAnyEvent();
                Continue();
            },
            this.stagedTargetedGemSlot.Description);

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
