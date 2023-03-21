using UnityEngine;
using System.Collections.Generic;
using BF2D.Game.Actions;
using System;
using BF2D.Game.Enums;
using BF2D.Enums;
using BF2D.UI;

namespace BF2D.Game.Combat
{
    [RequireComponent(typeof(Animator))]
    public class CharacterCombat : MonoBehaviour
    {
        private class AnimatorController
        {
            public bool HasEvent { get => this.animEvent is not null;  }

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

        public Actions.CombatAction CurrentCombatAction { get { return this.currentCombatAction; } set { this.currentCombatAction = value; } }
        private Actions.CombatAction currentCombatAction = null;

        public CombatGridTile Tile { set { this.assignedTile = value; } }
        private CombatGridTile assignedTile = null;

        public CharacterStats Stats { get { return this.stats; } set { this.stats = value; } }
        private CharacterStats stats = null;

        private void Awake()
        {
            this.animatorController = new AnimatorController(GetComponent<Animator>());
        }

        #region Public Utilities
        public void SetupCombatAction(Actions.CombatAction combatAction, UIControl targeter)
        {
            this.currentCombatAction = combatAction;
            if (this.Stats.CombatAI.Enabled)
                this.currentCombatAction.SetupAI(this.stats.CombatAI);
            else
                this.currentCombatAction.SetupControlled(targeter);
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
        private void StagePersistentEffectEvents(Action<PersistentEffect> stagingAction)
        {
            //Status Effect Event
            foreach (StatusEffectInfo info in this.Stats.StatusEffects)
            {
                stagingAction(info.Get());
            }

            //Equipment Event
            foreach (EquipmentType equipmentType in Enum.GetValues(typeof(EquipmentType)))
            {
                string equipmentID = this.Stats.GetEquipped(equipmentType);

                if (string.IsNullOrEmpty(equipmentID))
                    continue;

                Equipment equipment = GameInfo.Instance.GetEquipment(equipmentID);

                if (equipment is null)
                {
                    Terminal.IO.LogError($"[CharacterCombat:StageEquipmentUpkeep] Tried to get equipment at ID {equipmentID} but failed");
                    continue;
                }

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
                            RunTargetedGems(this.CurrentCombatAction.UseTargetedGems());
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

        private void StageUntargetedGems(CharacterStatsAction gem)
        {
            this.eventStack.PushEvent(() =>
            {
                this.animatorController.ChangeAnimState(Strings.Animation.Flashing, () =>
                {
                    PlayAnimation(gem.GetAnimationKey());

                    CharacterStatsAction.Info info = gem.Run(this.Stats, this.Stats);
                    List<string> dialog = GemStatusCheck(new List<CharacterStatsAction.Info> { info });
                    dialog.Insert(0, info.GetMessage());
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
                this.animatorController.ChangeAnimState(Strings.Animation.Attack, () =>
                {
                    List<CharacterStatsAction.Info> infos = new();
                    string message = string.Empty;

                    foreach (CharacterCombat target in action.TargetInfo.CombatTargets)
                    {
                        CharacterCombat verifiedTarget = target;
                        if (target.Stats.Dead)
                        {
                            if (action.Target == CharacterTarget.Self)
                            {
                                Terminal.IO.LogError("[CharacterCombat:StageTargetedGems] CRITICAL ERROR: Combat logic flawed");
                                continue;
                            }
                            else if (action.Target == CharacterTarget.Ally ||
                            action.Target == CharacterTarget.RandomAlly)
                            {
                                verifiedTarget = RollAlly();
                            }
                            else if (action.Target == CharacterTarget.Opponent ||
                            action.Target == CharacterTarget.RandomOpponent)
                            {
                                verifiedTarget = RollOpponent();
                            }
                            else if (action.Target == CharacterTarget.Any ||
                            action.Target == CharacterTarget.Random)
                            {
                                verifiedTarget = RollCharacterAligned(action.Gem.Alignment);
                            }
                            else if (action.Target == CharacterTarget.All ||
                            action.Target == CharacterTarget.AllOfAny ||
                            action.Target == CharacterTarget.AllAllies ||
                            action.Target == CharacterTarget.AllOpponents)
                            {
                                continue;
                            }
                        }

                        verifiedTarget.PlayAnimation(action.Gem.GetAnimationKey());

                        CharacterStatsAction.Info info = action.Gem.Run(this.stats, verifiedTarget.Stats);
                        message += info.GetMessage();
                        infos.Add(info);
                        verifiedTarget.RefreshStatsDisplay();
                    }

                    List<string> dialog = GemStatusCheck(infos);
                    dialog.Insert(0, message);
                    return dialog;
                });
            });
        }

        private CharacterCombat RollOpponent()
        {
            return CombatManager.Instance.CharacterIsEnemy(this) ?
            CombatManager.Instance.RandomPlayer() :
            CombatManager.Instance.RandomEnemy();
        }

        private CharacterCombat RollAlly()
        {
            return CombatManager.Instance.CharacterIsEnemy(this) ?
            CombatManager.Instance.RandomEnemy() :
            CombatManager.Instance.RandomPlayer();
        }

        private CharacterCombat RollCharacterAligned(CombatAlignment alignment)
        {
            if (CombatManager.Instance.CharacterIsEnemy(this))
            {
                return alignment == CombatAlignment.Offense ||
                alignment == CombatAlignment.Neutral ?
                CombatManager.Instance.RandomPlayer() :
                CombatManager.Instance.RandomEnemy();
            }
            else
            {
                return alignment == CombatAlignment.Offense ||
                alignment == CombatAlignment.Neutral ?
                CombatManager.Instance.RandomEnemy() :
                CombatManager.Instance.RandomPlayer();
            }
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
            //Finally, pass the turn
            this.eventStack.PushEvent(CombatManager.Instance.PassTurn);

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

        private void EOCEvent()
        {
            CombatManager.Instance.EndCombat();
        }
        #endregion

        #region Private Utilities
        private void FinalizeTurn()
        {
            this.eventStack.FlushEvents();
            this.currentCombatAction = null;
        }

        private void PlayDialog(List<string> dialog, Action callback)
        {
            dialog[^1] += "[E]";
            CombatManager.Instance.OrphanedTextbox.Dialog(dialog, false, 0, () =>
            {
                CombatManager.Instance.OrphanedTextbox.UtilityFinalize();
                callback?.Invoke();
            },
            new string[]
            {
                this.Stats.Name
            });
            CombatManager.Instance.OrphanedTextbox.View.gameObject.SetActive(true);
            CombatManager.Instance.OrphanedTextbox.UtilityInitialize();
        }

        private void PlayMessage(string message, Action callback)
        {
            CombatManager.Instance.OrphanedTextbox.Message(message, false, () =>
            {
                CombatManager.Instance.OrphanedTextbox.UtilityFinalize();
                callback?.Invoke();
            },
            new string[]
            {
                this.Stats.Name
            });
            CombatManager.Instance.OrphanedTextbox.View.gameObject.SetActive(true);
            CombatManager.Instance.OrphanedTextbox.UtilityInitialize();
        }
        #endregion
    }
}
