using UnityEngine;
using System.Collections.Generic;
using BF2D.Game.Actions;
using System;
using BF2D.Game.Enums;
using BF2D.UI;
using BF2D.Game.Combat.Actions;
using BF2D.Game.Combat.Enums;

namespace BF2D.Game.Combat
{
    public class CharacterCombat : MonoBehaviour, ICharacterController
    {
        private class EventStack
        {
            private readonly Stack<Action> eventStack = new();

            public void Continue()
            {
                if (this.eventStack.Count > 0)
                    this.eventStack.Pop()?.Invoke();
                else
                    Debug.LogWarning("[CharacterCombat:EventStack:Continue] Called while the stack was empty");
            }

            public void PushEvent(Action action)
            {
                this.eventStack.Push(action);
            }

            public void SuspendEvent(Action action)
            {
                this.eventStack.Push(() =>
                {
                    action?.Invoke();
                    Continue();
                });
            }

            public void FlushEvents()
            {
                this.eventStack.Clear();
            }
        }
        
        [Header("Control References")]
        [SerializeField] private SpriteRenderer spriteRenderer = null;
        [SerializeField] private AnimatorController animatorController = null;

        [Header("Preferences")] 
        [SerializeField] private float defaultMessageDelayDuration = 0.5f;
        private readonly EventStack stack = new();

        public CombatAction CurrentCombatAction => this.currentCombatAction;
        private CombatAction currentCombatAction = null;

        public CombatGridTile Tile { private get => this.assignedTile; set => this.assignedTile = value; }
        private CombatGridTile assignedTile = null;

        public CharacterStats Stats => this.characterInfo.Stats;
        public ICharacterInfo CharacterInfo { private get => this.characterInfo; set => this.characterInfo = value; }
        private ICharacterInfo characterInfo = null;

        #region Public Utilities
        public CharacterAlignment Alignment => this.Tile ? this.Tile.Alignment : CharacterAlignment.NPC;

        public bool IsPlayer => this.Alignment == CharacterAlignment.Player;
        public bool IsEnemy => this.Alignment == CharacterAlignment.Enemy;
        public bool IsNPC => this.Alignment == CharacterAlignment.NPC;

        public void DEBUG_CONTINUE()
        {
            this.stack.Continue();
        }

        public void SetupCombatAction(UIControl targeter, CombatAction combatAction)
        {
            this.currentCombatAction = combatAction;
            if (this.Stats.CombatAI.Enabled)
                this.currentCombatAction.SetupAI(this);
            else
                UICtx.One.TakeControl(targeter);
        }

        public void SetupCombatAction(CombatAction combatAction)
        {
            this.currentCombatAction = combatAction;
            if (this.Stats.CombatAI.Enabled)
                this.currentCombatAction.SetupAI(this);
            else
                this.currentCombatAction.SetupControlled();
        }

        public void Run()
        {
            this.stack.PushEvent(EOTEvent);                            //Finally, end the turn

            this.stack.SuspendEvent(() => this.Stats.ForEachPersistentEffect(StagePersistentEffectEOTEvent)); //Suspend the staging of EOT persistent effect events

            StageCombatEvents();

            this.Stats.ForEachPersistentEffect(StagePersistentEffectUpkeepEvent);  //Stage Upkeep persistent effect events

            CharacterStatusCheck();

            this.stack.Continue();
        }
        
        public void RunTargetedGameAction(TargetedGameAction gameAction, Action callback)
        {
            StageGameActionRunCallback(callback);
            this.Stats.CombatAI.SetupTargetedGameAction(gameAction, this);
            StageTargetedGameAction(gameAction);
            this.stack.Continue();
        }
        
        public void RunUntargetedGameAction(UntargetedGameAction gameAction, Action callback)
        {
            StageGameActionRunCallback(callback);
            StageUntargetedGameAction(gameAction);
            this.stack.Continue();
        }

        public void Destroy()
        {
            FinalizeTurn();

            if (this.Tile)
                this.Tile.ResetTile();

            if (this.CharacterInfo is not null && ReferenceEquals(this, this.CharacterInfo.CurrentController))
                this.CharacterInfo.CurrentController = null;

            Destroy(this.gameObject);
        }

        public void PlayAnimation(string key) => this.animatorController.ChangeAnimState(key);

        public void SetMaterial(Material material) => this.spriteRenderer.material = material;

        public void SetPosition(CombatGridTile destination, int sourceIndex, int destinationIndex)
        {
            if (!Numbers.ValidGridIndex(sourceIndex) || !Numbers.ValidGridIndex(destinationIndex))
            {
                Debug.LogError($"[CharacterCombat:SetPosition] One or both of the given indexes were outside of the range of the grid -> Source: {sourceIndex}, Destination: {destinationIndex}");
                return;
            }

            if (destination.Equals(this.Tile))
                return;

            if (!destination.AssignedCharacter || !this.Tile)
                SetPosition(destination, destinationIndex);
            else
                SwapPosition(destination, sourceIndex, destinationIndex);
        }

        public void SwapPosition(CombatGridTile destination, int sourceIndex, int destinationIndex)
        {
            if (!Numbers.ValidGridIndex(sourceIndex) || !Numbers.ValidGridIndex(destinationIndex))
            {
                Debug.LogError($"[CharacterCombat:SwapPosition] One or both of the given indexes were outside of the range of the grid -> Source: {sourceIndex}, Destination: {destinationIndex}");
                return;
            }

            if (destination.Equals(this.Tile))
                return;

            destination.AssignedCharacter.CharacterInfo.Position = sourceIndex;
            this.Tile.AssignCharacter(destination.AssignedCharacter);
            destination.ResetTile();

            SetPosition(destination, destinationIndex);
        }

        public void SetPosition(CombatGridTile destination, int destinationIndex)
        {
            if (!Numbers.ValidGridIndex(destinationIndex))
            {
                Debug.LogError($"[CharacterCombat:SwapPosition] The index was outside of the range of the grid -> {destinationIndex}");
                return;
            }

            if (destination.Equals(this.Tile))
                return;

            if (destination.AssignedCharacter)
                destination.AssignedCharacter.Destroy();

            destination.AssignCharacter(this);
            this.CharacterInfo.Position = destinationIndex;
        }
        #endregion

        #region Animation Events
        public void AnimTrigger()
        {
            if (!this.animatorController.HasEvent)
                return;

            List<string> message = this.animatorController.InvokeAnimEvent();
            PlayDialog(message, this.stack.Continue);
        }

        public void AnimSwitchIdle() => this.animatorController.ChangeAnimState(Strings.Animation.IDLE_ID);
        #endregion

        //Private Methods

        #region Stage and Run Persistent Effect Events
        private void StagePersistentEffectUpkeepEvent(PersistentEffect persistentEffect)
        {
            if (persistentEffect.UpkeepEventExists)
                StagePersistentEffectEvent(persistentEffect.OnUpkeep);
        }

        private void StagePersistentEffectEOTEvent(PersistentEffect persistentEffect)
        {
            if (persistentEffect.EOTEventExists)
                StagePersistentEffectEvent(persistentEffect.OnEOT);
        }

        private void StagePersistentEffectEvent(UntargetedGameAction gameAction) => 
            this.stack.PushEvent(() => 
            PlayDialog(gameAction.GetMessage(), () =>
        {
            StageUntargetedGems(gameAction.GemSlots);
            this.stack.Continue();
        }));
        #endregion

        #region Stage General Combat Events
        private void StageCombatEvents() =>
            this.stack.PushEvent(() =>
        {
            switch (this.CurrentCombatAction.Type)
            {
                case CombatActionType.Item:
                    StageTargetedGameAction(this.CurrentCombatAction.UseTargetedGameAction());
                    this.stack.Continue();
                    return;
                case CombatActionType.Equip:
                    StageUntargetedGameAction(this.CurrentCombatAction.GetUntargetedGameAction());
                    PlayDialog(this.CurrentCombatAction.Run(), this.stack.Continue);
                    return;
                default:
                    throw new NotImplementedException();
            }
        });
        #endregion
        
        #region Stage and Run GameActions
        private void StageUntargetedGameAction(UntargetedGameAction gameAction)
        {
            StageUntargetedGems(gameAction.GemSlots);
            StageGameActionMessage(gameAction);
        }
        
        private void StageTargetedGameAction(TargetedGameAction gameAction)
        {
            StageTargetedGems(gameAction.TargetedGemSlots);
            StageGameActionMessage(gameAction);
        }

        private void StageGameActionMessage(GameAction gameAction)
        {
            List<string> dialog = gameAction.GetMessage();
            float? messageDelayDuration = gameAction.MessageDelayDuration;

            if (messageDelayDuration is null)
            {
                if (dialog.Count > 1)
                    dialog = DialogTextbox.AppendDelayToEachLine(dialog, this.defaultMessageDelayDuration);
            }
            else 
                dialog = DialogTextbox.AppendDelayToEachLine(dialog, messageDelayDuration.GetValueOrDefault());
            
            dialog = DialogTextbox.AppendEndTag(dialog);
            if (dialog is not null)
                this.stack.PushEvent(() => PlayDialog(dialog, this.stack.Continue));
        }
        
        private void StageGameActionRunCallback(Action callback) =>
            this.stack.PushEvent(() =>
        {
            callback?.Invoke();
            CombatCtx.One.OrphanedTextbox.View.gameObject.SetActive(false);
        });
        #endregion

        #region Stage and Run Gems
        private void StageUntargetedGems(IEnumerable<CharacterActionSlot> gemSlots)
        {
            foreach (CharacterActionSlot slot in gemSlots)
                StageUntargetedGem(slot);
        }

        private void StageUntargetedGem(CharacterActionSlot slot) =>
            this.stack.PushEvent(() =>
            this.animatorController.ChangeAnimState(Strings.Animation.FLASHING_ID, () =>
        {
            PlayAnimation(slot.GetAnimationKey());

            CharacterAction.RunInfo runInfo = slot.Run(this.Stats);
            List<string> dialog = GemStatusCheck(new List<CharacterAction.RunInfo> { runInfo });
            dialog.Insert(0, runInfo.GetMessage());
            RefreshStatsDisplay();
            return dialog;
        }));

        private void StageTargetedGems(IEnumerable<TargetedCharacterActionSlot> targetedGemSlots)
        {
            foreach (TargetedCharacterActionSlot targetedGemSlot in targetedGemSlots)
                StageTargetedGem(targetedGemSlot);
        }

        private void StageTargetedGem(TargetedCharacterActionSlot slot) =>
            this.stack.PushEvent(() =>
            this.animatorController.ChangeAnimState(Strings.Animation.ATTACK_ID, () =>
        {
            List<CharacterAction.RunInfo> infos = new();
            string message = string.Empty;

            foreach (CharacterCombat target in slot.TargetInfo.CombatTargets)
            {
                //Verify that targets are still valid before executing
                CharacterCombat verifiedTarget = target;
                if (target.Stats.Dead)
                {
                    if (slot.Target == CharacterTarget.Self)
                    {
                        Debug.LogError($"[CharacterCombat:StageTargetedGems] CRITICAL ERROR: Combat logic flawed. Somehow, ${target.Stats.Name} tried to target themself while they were dead.");
                        continue;
                    }
                    
                    if (slot.Target == CharacterTarget.Ally ||
                    slot.Target == CharacterTarget.RandomAlly)
                    {
                        verifiedTarget = RollAlly();
                    }
                    else if (slot.Target == CharacterTarget.Opponent ||
                    slot.Target == CharacterTarget.RandomOpponent)
                    {
                        verifiedTarget = RollOpponent();
                    }
                    else if (slot.Target == CharacterTarget.Any ||
                    slot.Target == CharacterTarget.Random)
                    {
                        verifiedTarget = RollCharacterAligned(slot.Alignment);
                    }
                    else if (slot.Target == CharacterTarget.All ||
                    slot.Target == CharacterTarget.AllOfAny ||
                    slot.Target == CharacterTarget.AllAllies ||
                    slot.Target == CharacterTarget.AllOpponents)
                    {
                        continue;
                    }
                }

                CharacterAction.RunInfo runInfo = slot.Run(this.Stats, verifiedTarget.Stats);

                if (!runInfo.failed && !runInfo.insufficientStamina)
                {
                    //Execute   
                    verifiedTarget.PlayAnimation(slot.GetAnimationKey());
                }

                message += runInfo.GetMessage();
                infos.Add(runInfo);
                verifiedTarget.RefreshStatsDisplay();
                RefreshStatsDisplay();  //for exertion cost
            }

            List<string> dialog = GemStatusCheck(infos);
            dialog.Insert(0, message);
            return dialog;
        }));

        private CharacterCombat RollOpponent() => CombatCtx.One.RandomOpponent(this.Alignment);

        private CharacterCombat RollAlly() => CombatCtx.One.RandomAlly(this.Alignment);

        private CharacterCombat RollCharacterAligned(CombatAlignment combatAlignment) =>
            combatAlignment == CombatAlignment.Offense || combatAlignment == CombatAlignment.Neutral ?
            RollOpponent() :
            RollAlly();

        private List<string> GemStatusCheck(IEnumerable<CharacterAction.RunInfo> infos)
        {
            bool iDied = false;
            List<string> dialog = new();
            foreach (CharacterAction.RunInfo info in infos)
            {
                if (info.targetWasKilled)
                {
                    if (info.target == this.Stats)
                        iDied = true;

                    dialog.Add($"{info.target.Name} died. {Strings.DialogTextbox.PAUSE_STANDARD}");
                }
            }

            return StatusCheckDispatch(dialog, iDied);
        }

        private void CharacterStatusCheck()
        {
            bool iDied = false;
            List<string> dialog = new();
            foreach (CharacterCombat character in CombatCtx.One.AllCharacters)
            {
                if (character.Stats.Dead)
                {
                    if (character.Stats == this.Stats)
                        iDied = true;

                    dialog.Add($"{character.Stats.Name} died. {Strings.DialogTextbox.PAUSE_STANDARD}");
                }
            }

            dialog = StatusCheckDispatch(dialog, iDied);
            if (dialog.Count > 0)
                this.stack.PushEvent(() => PlayDialog(dialog, this.stack.Continue));
        }

        private List<string> StatusCheckDispatch(List<string> dialog, bool iDied)
        {
            //No one died
            if (dialog.Count < 1)
                return new List<string>();

            if (CombatCtx.One.CombatIsOver() || iDied)
                FinalizeTurn();

            if (CombatCtx.One.CombatIsOver())
                this.stack.PushEvent(EOCEvent);

            if (iDied && !CombatCtx.One.CombatIsOver())
                this.stack.PushEvent(EOTEvent);

            return dialog;
        }
        #endregion

        #region EOC and EOT
        private void EOTEvent()
        {
            //Finally, pass the turn
            this.stack.PushEvent(CombatCtx.One.PassTurn);

            //Cleanup status effects
            foreach (StatusEffectInfo info in this.Stats.StatusEffects)
            {
                StatusEffect statusEffect = info.Use();
                if (info.RemainingDuration == 0)
                {
                    this.stack.PushEvent(() =>
                    {
                        this.Stats.RemoveStatusEffect(info);
                        PlayMessage($"The {statusEffect.Name} on {this.Stats.Name} wore off. {Strings.DialogTextbox.PAUSE_BREIF}", this.stack.Continue);
                    });
                }
            }

            this.stack.Continue();
        }

        private void EOCEvent() => CombatCtx.One.EndCombat();
        #endregion

        #region Private Utilities
        private void RefreshStatsDisplay()
        {
            this.Tile.SetHealthBar();
            this.Tile.SetStaminaBar();
        }
        
        private void FinalizeTurn()
        {
            this.stack.FlushEvents();
            this.currentCombatAction = null;
        }

        private void PlayDialog(List<string> dialog, Action callback)
        {
            DialogTextbox textbox = CombatCtx.One.OrphanedTextbox;
            dialog[^1] += Strings.DialogTextbox.END;
            
            textbox.Dialog
            (
                lines: dialog,
                startingLine: 0, 
                callback: () =>
                {
                    FinalizeTextbox(textbox);
                    callback?.Invoke();
                },
                inserts: this.Stats.Name
            );

            InitializeTextbox(textbox);
        }

        private void PlayMessage(string message, Action callback)
        {
            DialogTextbox textbox = CombatCtx.One.OrphanedTextbox;

            textbox.Message(message, () =>
            {
                FinalizeTextbox(textbox);
                callback?.Invoke();
            },
            this.Stats.Name);
            
            InitializeTextbox(textbox);
        }

        private void InitializeTextbox(DialogTextbox textbox)
        {
            textbox.autoPass = true;
            textbox.messageInterrupt = false;
            textbox.UtilityInitialize();
        }

        private void FinalizeTextbox(DialogTextbox textbox)
        {
            textbox.autoPass = default;
            textbox.messageInterrupt = default;
            textbox.UtilityFinalize();
        }
        #endregion
    }
}
