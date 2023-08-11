using UnityEngine;
using System.Collections.Generic;
using BF2D.Game.Actions;
using System;
using BF2D.Game.Enums;
using BF2D.Enums;
using BF2D.UI;

namespace BF2D.Game.Combat
{
    public class CharacterCombat : MonoBehaviour
    {
        private class EventStack
        {
            private readonly Stack<Action> eventStack = new();

            public void Continue()
            {
                if (this.eventStack.Count > 0)
                    this.eventStack.Pop()();
                else
                    Debug.LogWarning("[CharacterCombat:EventStack:Continue] Called while the stack was empty");
            }

            public void PushEvent(Action action)
            {
                this.eventStack.Push(action);
            }

            public void FlushEvents()
            {
                this.eventStack.Clear();
            }
        }

        [SerializeField] private SpriteRenderer spriteRenderer = null;
        [SerializeField] private AnimatorController animatorController = null;
        private readonly EventStack eventStack = new();

        public Actions.CombatAction CurrentCombatAction { get => this.currentCombatAction; set => this.currentCombatAction = value; }
        private Actions.CombatAction currentCombatAction = null;

        public CombatGridTile Tile { set => this.assignedTile = value; }
        private CombatGridTile assignedTile = null;

        public CharacterStats Stats { get => this.stats; set => this.stats = value; }
        private CharacterStats stats = null;

        #region Public Utilities
        public void SetupCombatAction(UIControl targeter, Actions.CombatAction combatAction)
        {
            this.currentCombatAction = combatAction;
            if (this.Stats.CombatAI.Enabled)
                this.currentCombatAction.SetupAI(this.stats.CombatAI);
            else
                this.currentCombatAction.SetupControlled(targeter);
        }

        public void Run()
        {
            this.eventStack.PushEvent(EOTEvent);                            //Finally, end the turn

            PersistentEffectEventLoader(StagePersistentEffectEOTEvent);     //Stage EOT persistent effect events

            StageCombatEvents();

            PersistentEffectEventLoader(StagePersistentEffectUpkeepEvent);  //Stage Upkeep persistent effect events

            this.eventStack.Continue();                                     //Run Combat
        }

        public void Destroy()
        {
            FinalizeTurn();

            if (this.assignedTile)
                this.assignedTile.ResetTile();

            Destroy(this.gameObject);
        }

        public void PlayAnimation(string key) => this.animatorController.ChangeAnimState(key);

        public void SetMaterial(Material material) => this.spriteRenderer.material = material;

        private void RefreshStatsDisplay()
        {
            this.assignedTile.SetHealthBar();
            this.assignedTile.SetStaminaBar();
        }
        #endregion

        #region Animation Events
        public void AnimTrigger()
        {
            if (!this.animatorController.HasEvent)
                return;

            List<string> message = this.animatorController.InvokeAnimEvent();
            PlayDialog(message, this.eventStack.Continue);
        }

        public void AnimSwitchIdle() => this.animatorController.ChangeAnimState(Strings.Animation.Idle);
        #endregion

        //Private Methods

        #region Stage and Run Persistent Effect Events
        private void PersistentEffectEventLoader(Action<PersistentEffect> stagingAction)
        {
            //Status Effect Event
            foreach (StatusEffectInfo info in this.Stats.StatusEffects)
                stagingAction(info.Get());

            //Equipment Event
            foreach (EquipmentType equipmentType in Enum.GetValues(typeof(EquipmentType)))
            {
                Equipment equipment = this.Stats.GetEquipped(equipmentType);

                if (equipment is null)
                    continue;

                stagingAction(equipment);
            }
        }

        private void StagePersistentEffectUpkeepEvent(PersistentEffect persistentEffect)
        {
            if (persistentEffect.UpkeepEventExists())
                this.eventStack.PushEvent(() => PlayPersistentEffectEvent(persistentEffect.OnUpkeep));
        }

        private void StagePersistentEffectEOTEvent(PersistentEffect persistentEffect)
        {
            if (persistentEffect.EOTEventExists())
                this.eventStack.PushEvent(() => PlayPersistentEffectEvent(persistentEffect.OnEOT));
        }

        private void PlayPersistentEffectEvent(UntargetedGameAction gameAction)
        {
            PlayDialog(gameAction.Message, () => RunUntargetedGems(gameAction.GemSlots));
        }
        #endregion

        #region Stage General Combat Events
        private void StageCombatEvents() => this.eventStack.PushEvent(() =>
        {
            PlayDialog(this.CurrentCombatAction.CurrentInfo.GetOpeningMessage(), () =>
            {
                switch (this.CurrentCombatAction.Type)
                {
                    case Enums.CombatActionType.Item:
                        RunTargetedGems(this.CurrentCombatAction.UseTargetedGemSlots());
                        break;
                    default:
                        throw new NotImplementedException();
                }
            });
        });
        #endregion

        #region Stage and Run Gems
        private void RunUntargetedGems(IEnumerable<CharacterStatsActionSlot> gemSlots)
        {
            foreach (CharacterStatsActionSlot slot in gemSlots)
                StageUntargetedGems(slot);

            this.eventStack.Continue();
        }

        private void StageUntargetedGems(CharacterStatsActionSlot slot) => this.eventStack.PushEvent(() =>
        {
            this.animatorController.ChangeAnimState(Strings.Animation.Flashing, () =>
            {
                PlayAnimation(slot.GetAnimationKey());

                CharacterStatsAction.Info info = slot.Run(this.Stats);
                List<string> dialog = GemStatusCheck(new List<CharacterStatsAction.Info> { info });
                dialog.Insert(0, info.GetMessage());
                RefreshStatsDisplay();
                return dialog;
            });
        });

        private void RunTargetedGems(IEnumerable<TargetedCharacterStatsActionSlot> targetedGemSlots)
        {
            foreach (TargetedCharacterStatsActionSlot targetedGemSlot in targetedGemSlots)
                StageTargetedGems(targetedGemSlot);

            this.eventStack.Continue();
        }

        private void StageTargetedGems(TargetedCharacterStatsActionSlot targetedGemSlot) => this.eventStack.PushEvent(() =>
        {
            this.animatorController.ChangeAnimState(Strings.Animation.Attack, () =>
            {
                List<CharacterStatsAction.Info> infos = new();
                string message = string.Empty;

                foreach (CharacterCombat target in targetedGemSlot.TargetInfo.CombatTargets)
                {
                    //Verify that targets are still valid before executing
                    CharacterCombat verifiedTarget = target;
                    if (target.Stats.Dead)
                    {
                        if (targetedGemSlot.Target == CharacterTarget.Self)
                        {
                            Debug.LogError($"[CharacterCombat:StageTargetedGems] CRITICAL ERROR: Combat logic flawed. Somehow, ${target.Stats.Name} tried to target themself while they were dead.");
                            continue;
                        }
                        else if (targetedGemSlot.Target == CharacterTarget.Ally ||
                        targetedGemSlot.Target == CharacterTarget.RandomAlly)
                        {
                            verifiedTarget = RollAlly();
                        }
                        else if (targetedGemSlot.Target == CharacterTarget.Opponent ||
                        targetedGemSlot.Target == CharacterTarget.RandomOpponent)
                        {
                            verifiedTarget = RollOpponent();
                        }
                        else if (targetedGemSlot.Target == CharacterTarget.Any ||
                        targetedGemSlot.Target == CharacterTarget.Random)
                        {
                            verifiedTarget = RollCharacterAligned(targetedGemSlot.Alignment);
                        }
                        else if (targetedGemSlot.Target == CharacterTarget.All ||
                        targetedGemSlot.Target == CharacterTarget.AllOfAny ||
                        targetedGemSlot.Target == CharacterTarget.AllAllies ||
                        targetedGemSlot.Target == CharacterTarget.AllOpponents)
                        {
                            continue;
                        }
                    }

                    //Execute
                    verifiedTarget.PlayAnimation(targetedGemSlot.GetAnimationKey());

                    CharacterStatsAction.Info info = targetedGemSlot.Run(this.stats, verifiedTarget.Stats);
                    message += info.GetMessage();
                    infos.Add(info);
                    verifiedTarget.RefreshStatsDisplay();
                }

                List<string> dialog = GemStatusCheck(infos);
                dialog.Insert(0, message);
                return dialog;
            });
        });

        private CharacterCombat RollOpponent()
        {
            return CombatCtx.One.CharacterIsEnemy(this) ?
            CombatCtx.One.RandomPlayer() :
            CombatCtx.One.RandomEnemy();
        }

        private CharacterCombat RollAlly()
        {
            return CombatCtx.One.CharacterIsEnemy(this) ?
            CombatCtx.One.RandomEnemy() :
            CombatCtx.One.RandomPlayer();
        }

        private CharacterCombat RollCharacterAligned(CombatAlignment alignment)
        {

            bool aligned = alignment == CombatAlignment.Offense ||
                           alignment == CombatAlignment.Neutral;

            if (CombatCtx.One.CharacterIsEnemy(this))
                return aligned ?
                CombatCtx.One.RandomPlayer() :
                CombatCtx.One.RandomEnemy();
            else
                return aligned ?
                CombatCtx.One.RandomEnemy() :
                CombatCtx.One.RandomPlayer();
        }

        private List<string> GemStatusCheck(IEnumerable<CharacterStatsAction.Info> infos)
        {
            bool iDied = false;
            List<string> dialog = new();
            foreach (CharacterStatsAction.Info info in infos)
            {
                if (info.targetWasKilled)
                {
                    if (info.target == this.Stats)
                        iDied = true;

                    dialog.Add($"{info.target.Name} died. {Strings.DialogTextbox.StandardPause}");
                }
            }

            //No one died
            if (dialog.Count < 1)
                return new List<string>();

            if (CombatCtx.One.CombatIsOver() || iDied)
                FinalizeTurn();

            if (CombatCtx.One.CombatIsOver())
                this.eventStack.PushEvent(EOCEvent);

            if (iDied && !CombatCtx.One.CombatIsOver())
                this.eventStack.PushEvent(EOTEvent);

            return dialog;
        }
        #endregion

        #region EOC and EOT
        private void EOTEvent()
        {
            //Finally, pass the turn
            this.eventStack.PushEvent(CombatCtx.One.PassTurn);

            //Cleanup status effects
            foreach (StatusEffectInfo info in this.Stats.StatusEffects)
            {
                StatusEffect statusEffect = info.Use();
                if (info.RemainingDuration == 0)
                {
                    this.eventStack.PushEvent(() =>
                    {
                        this.Stats.RemoveStatusEffect(info);
                        PlayMessage($"The {statusEffect.Name} on {this.Stats.Name} wore off. {Strings.DialogTextbox.BriefPause}", this.eventStack.Continue);
                    });
                }
            }

            this.eventStack.Continue();
        }

        private void EOCEvent() => CombatCtx.One.EndCombat();
        #endregion

        #region Private Utilities
        private void FinalizeTurn()
        {
            this.eventStack.FlushEvents();
            this.currentCombatAction = null;
        }

        private void PlayDialog(List<string> dialog, Action callback)
        {
            DialogTextbox textbox = CombatCtx.One.OrphanedTextbox;
            dialog[^1] += Strings.DialogTextbox.End;

            textbox.Dialog(dialog, 0, () =>
            {
                FinalizeTextbox(textbox);
                callback?.Invoke();
            },
            new string[]
            {
                this.Stats.Name
            });

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
            new string[]
            {
                this.Stats.Name
            });

            InitializeTextbox(textbox);
        }

        private void InitializeTextbox(DialogTextbox textbox)
        {
            textbox.View.gameObject.SetActive(true);
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
