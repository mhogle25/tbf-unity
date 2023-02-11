using UnityEngine;
using BF2D.Game;
using BF2D.Combat.Actions;
using BF2D.UI;
using System.Collections.Generic;
using BF2D.Game.Actions;
using System;
using BF2D.Game.Enums;
using System.Runtime.CompilerServices;
using System.Numerics;

namespace BF2D.Combat
{
    [RequireComponent(typeof(Animator))]
    public class CharacterCombat : MonoBehaviour
    {
        private class AnimatorController
        {
            public bool HasEvent { get { return this.animEvent is not null; } }

            private string animState = Strings.Animation.Idle;
            public delegate List<string> RunEvent();
            private RunEvent animEvent = null;

            private readonly Animator animator = null;

            public AnimatorController(Animator animator)
            {
                this.animator = animator;
            }

            public void ChangeAnimState(string newState)
            {
                ChangeAnimState(newState, null);
            }

            public void ChangeAnimState(string newState, RunEvent callback)
            {
                if (this.animState == newState) return;
                this.animator.Play(newState);
                this.animState = newState;
                this.animEvent = callback;
            }

            public List<string> InvokeAnimEvent()
            {
                List<string> dialog = null;
                dialog = this.animEvent?.Invoke();
                this.animEvent = null;
                return dialog;
            }
        }

        private AnimatorController animatorController = null;

        private class EventStack
        {
            private readonly Stack<Action> eventStack = new();

            public void Continue()
            {
                if (this.eventStack.Count > 0)
                    this.eventStack.Pop()?.Invoke();
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

        private readonly EventStack eventStack = new();

        public CombatAction CurrentCombatAction { get { return this.currentCombatAction; } }
        private CombatAction currentCombatAction = null;

        public CombatGridTile Tile { set { this.assignedTile = value; } }
        private CombatGridTile assignedTile = null;

        public CharacterStats Stats { get { return this.stats; } set { this.stats = value; } }
        private CharacterStats stats = null;

        private void Awake()
        {
            this.animatorController = new AnimatorController(GetComponent<Animator>());
        }

        #region Public Utilities
        public void SetupCombatAction(CombatAction combatAction)
        {
            this.currentCombatAction = combatAction;
            this.currentCombatAction.Setup();
        }

        public void RunCombatEvents()
        {
            this.eventStack.PushEvent(EOTEvent);

            StagePersistentEffectEvents(StagePersistentEffectEOTEvent);     //Stage EOT persistent effect events

            StageCombatEvents();

            StagePersistentEffectEvents(StagePersistentEffectUpkeepEvent);  //Stage Upkeep persistent effect events

            //Run Combat
            this.eventStack.Continue();
        }

        public void Destroy()
        {
            FinalizeTurn();

            if (this.assignedTile)
                this.assignedTile.ResetTile();

            Destroy(this.gameObject);
        }

        public void PlayAnimation(string key)
        {
            this.animatorController.ChangeAnimState(key);
        }

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

        public void AnimSwitchIdle()
        {
            this.animatorController.ChangeAnimState(BF2D.Game.Strings.Animation.Idle);
        }
        #endregion

        //Private Methods

        #region Stage and Run Persistent Effect Events
        private void StagePersistentEffectEvents(Action<PersistentEffect, Action> stagingAction)
        {
            //Status Effect Event
            foreach (StatusEffectInfo info in this.Stats.StatusEffects)
            {
                stagingAction(info.Get(), () => 
                {
                    info.Use();
                    if (info.RemainingDuration == 0)
                    {
                        this.eventStack.PushEvent(() =>
                        {
                            this.Stats.RemoveStatusEffect(info);
                            PlayMessage($"The {info.Get().Name} on {this.Stats.Name} wore off. [P:0.1]", this.eventStack.Continue);
                        });

                    }
                });
            }

            //Equipment Event
            foreach (EquipmentType equipmentType in Enum.GetValues(typeof(EquipmentType)))
            {
                string equipmentID = this.Stats.GetEquipped(equipmentType);

                if (equipmentID == string.Empty || equipmentID is null)
                    continue;

                Equipment equipment = GameInfo.Instance.GetEquipment(equipmentID);

                if (equipment is null)
                {
                    Terminal.IO.LogError($"[CharacterCombat:StageEquipmentUpkeep] Tried to get equipment at ID {equipmentID} but failed");
                    continue;
                }

                stagingAction(equipment, null);
            }
        }

        private void StagePersistentEffectUpkeepEvent(PersistentEffect persistentEffect, Action callback)
        {
            if (persistentEffect.UpkeepEventExists())
                this.eventStack.PushEvent(() =>
                {
                    callback?.Invoke();
                    PlayPersistentEffectEvent(persistentEffect.OnUpkeep);
                });
        }

        private void StagePersistentEffectEOTEvent(PersistentEffect persistentEffect, Action callback)
        {
            if (persistentEffect.EOTEventExists())
                this.eventStack.PushEvent(() =>
                {
                    callback?.Invoke();
                    PlayPersistentEffectEvent(persistentEffect.OnEOT);
                });
        }

        private void PlayPersistentEffectEvent(UntargetedGameAction gameAction)
        {
            PlayDialog(gameAction.Message, () =>
            {
                RunUntargetedGems(gameAction.Gems);
            });
        }
        #endregion

        #region Stage General Combat Events
        private void StageCombatEvents()
        {
            this.eventStack.PushEvent(() =>
            {
                PlayDialog(this.CurrentCombatAction.CurrentInfo.GetOpeningMessage(), () =>
                {
                    switch (this.CurrentCombatAction.CombatActionType)
                    {
                        case Enums.CombatActionType.Item:
                            RunTargetedGems(this.CurrentCombatAction.UseTargetedStatsActions());
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                });
            });
        }
        #endregion

        #region Stage and Run Gems
        private void RunUntargetedGems(IEnumerable<CharacterStatsAction> actions)
        {
            foreach (CharacterStatsAction action in actions)
            {
                StageUntargetedGems(action);
            }

            this.eventStack.Continue();
        }

        private void StageUntargetedGems(CharacterStatsAction action)
        {
            this.eventStack.PushEvent(() =>
            {
                this.animatorController.ChangeAnimState(Strings.Animation.Flashing, () =>
                {
                    PlayAnimation(action.GetAnimationKey());

                    CharacterStatsAction.Info info = action.Run(this.Stats, this.Stats);
                    List<string> dialog = GemStatusCheck(new List<CharacterStatsAction.Info> { info });
                    dialog.Insert(0, info.message);
                    RefreshStatsDisplay();
                    return dialog;
                });
            });
        }

        private void RunTargetedGems(IEnumerable<TargetedCharacterStatsAction> actions)
        {
            foreach (TargetedCharacterStatsAction action in actions)
            {
                StageTargetedGems(action);
            }

            this.eventStack.Continue();
        }

        private void StageTargetedGems(TargetedCharacterStatsAction action)
        {
            this.eventStack.PushEvent(() =>
            {
                const string DEFAULT_MESSAGE = "But no one was affected.";

                this.animatorController.ChangeAnimState(Strings.Animation.Attack, () =>
                {
                    List<CharacterStatsAction.Info> infos = new();
                    string message = DEFAULT_MESSAGE;

                    foreach (CharacterCombat target in action.TargetInfo.CombatTargets)
                    {
                        if (target.Stats.Dead)
                            continue;

                        target.PlayAnimation(action.Gem.GetAnimationKey());

                        if (message == DEFAULT_MESSAGE)
                            message = string.Empty;
                        CharacterStatsAction.Info info = action.Gem.Run(this.stats, target.Stats);
                        message += info.message;
                        infos.Add(info);
                        target.RefreshStatsDisplay();
                    }

                    List<string> dialog = GemStatusCheck(infos);
                    dialog.Insert(0, message);
                    return dialog;
                });
            });
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

                    dialog.Add($"{info.target.Name} died. [P:0.1]");
                }
            }

            //No one died
            if (dialog.Count < 1)
                return new List<string>();

            if (CombatManager.Instance.CombatIsOver() || iDied)
                FinalizeTurn();

            if (CombatManager.Instance.CombatIsOver())
                this.eventStack.PushEvent(EOCEvent);

            if (iDied && !CombatManager.Instance.CombatIsOver())
                this.eventStack.PushEvent(EOTEvent);

            return dialog;
        }
        #endregion

        #region EOC and EOT
        private void EOTEvent()
        {
            Terminal.IO.Log("End of Turn");
            CombatManager.Instance.PassTurn();
        }

        private void EOCEvent()
        {
            Terminal.IO.Log("End of Combat");
            CombatManager.Instance.EndCombat();
        }
        #endregion

        #region Private Utilities
        private void FinalizeTurn()
        {
            this.eventStack.FlushEvents();
            this.currentCombatAction = null;
        }

        private void PlayDialog(List<string> dialog)
        {
            PlayDialog(dialog, null);
        }

        private void PlayDialog(List<string> dialog, Action callback)
        {
            dialog[dialog.Count - 1] += "[E]";
            CombatManager.Instance.OrphanedTextbox.Dialog(dialog, 0, () =>
            {
                CombatManager.Instance.OrphanedTextbox.UtilityFinalize();
                callback?.Invoke();
            },
            new List<string>
            {
                this.Stats.Name
            });
            CombatManager.Instance.OrphanedTextbox.View.gameObject.SetActive(true);
            CombatManager.Instance.OrphanedTextbox.UtilityInitialize();
        }

        private void PlayMessage(string message)
        {
            PlayMessage(message, null);
        }

        private void PlayMessage(string message, Action callback)
        {
            CombatManager.Instance.OrphanedTextbox.Message(message, () =>
            {
                CombatManager.Instance.OrphanedTextbox.UtilityFinalize();
                callback?.Invoke();
            },
            new List<string>
            {
                this.Stats.Name
            });
            CombatManager.Instance.OrphanedTextbox.View.gameObject.SetActive(true);
            CombatManager.Instance.OrphanedTextbox.UtilityInitialize();
        }
        #endregion
    }
}
