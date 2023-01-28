using UnityEngine;
using BF2D.Game;
using BF2D.Combat.Actions;
using BF2D.UI;
using System.Collections.Generic;
using BF2D.Game.Actions;
using System;
using BF2D.Game.Enums;
using System.Runtime.CompilerServices;

namespace BF2D.Combat
{
    [RequireComponent(typeof(Animator))]
    public class CharacterCombat : MonoBehaviour
    {
        private class AnimatorController
        {
            public bool HasEvent { get { return this.animEvent is not null; } }

            private string animState = Strings.Animation.Idle;
            public delegate string RunEvent();
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

            public string InvokeAnimEvent()
            {
                string message = string.Empty;
                message = this.animEvent?.Invoke();
                this.animEvent = null;
                return message;
            }
        }

        private AnimatorController animatorController = null;

        private readonly Stack<Action> eventStack = new();

        public CombatAction CurrentCombatAction { get { return this.currentCombatAction; } }
        private CombatAction currentCombatAction = null;

        public CombatGridTile Tile { set { this.assignedTile = value; } }
        private CombatGridTile assignedTile = null;

        public CharacterStats Stats { get { return this.stats; } set { this.stats = value; } }
        private CharacterStats stats;

        private void Awake()
        {
            this.animatorController = new AnimatorController(GetComponent<Animator>());
        }

        #region Combat Event Stack
        private void Continue()
        {
            if (this.eventStack.Count > 0)
                this.eventStack.Pop()?.Invoke();
        }

        private void PushEvent(Action action)
        {
            this.eventStack.Push(() =>
            {
                if (this.Stats.Dead)
                {
                    DeathEvent();
                    return;
                }

                action?.Invoke();
            });
        }

        private void FlushEvents()
        {
            this.eventStack.Clear();
        }
        #endregion

        #region Public Utilities
        public void SetupCombatAction(CombatAction combatAction)
        {
            this.currentCombatAction = combatAction;
            this.currentCombatAction.Setup();
        }

        public void RunCombat()
        {
            PushEvent(() =>
            {
                PlayMessage("This is when the final event of this character's turn should be triggered", () =>
                {
                    Debug.Log("Trigger");
                });
            });

            StagePersistentEffectEvents(StagePersistentEffectEOTEvent);     //Stage EOT events

            StageCombatEvents();

            StagePersistentEffectEvents(StagePersistentEffectUpkeepEvent);  //Stage Upkeep Events

            //Run Combat
            Continue();
        }

        public void Destroy()
        {
            if (this.assignedTile)
                this.assignedTile.ResetTile();

            Destroy(this.gameObject);
        }

        public void PlayAnimation(string key)
        {
            this.animatorController.ChangeAnimState(key);
        }
        #endregion

        #region Animation Events
        public void AnimTrigger()
        {
            if (!this.animatorController.HasEvent)
                return;

            string message = this.animatorController.InvokeAnimEvent();
            PlayMessage(message, Continue);
        }

        public void AnimSwitchIdle()
        {
            this.animatorController.ChangeAnimState(BF2D.Game.Strings.Animation.Idle);
        }
        #endregion

        #region Stage Persistent Effect Events
        private void StagePersistentEffectEvents(Action<PersistentEffect> stagingAction)
        {
            //Status Effect Event
            foreach (StatusEffect statusEffect in this.Stats.StatusEffects)
            {
                stagingAction(statusEffect);
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
                    Debug.LogError($"[CharacterCombat:StageEquipmentUpkeep] Tried to get equipment at ID {equipmentID} but failed");
                    continue;
                }

                stagingAction(equipment);
            }
        }

        private void StagePersistentEffectUpkeepEvent(PersistentEffect persistentEffect)
        {
            if (persistentEffect.UpkeepEventExists())
                PushEvent(() =>
                {
                    PlayPersistentEffectEvent(persistentEffect.OnUpkeep);
                });
        }

        private void StagePersistentEffectEOTEvent(PersistentEffect persistentEffect)
        {
            if (persistentEffect.EOTEventExists())
                PushEvent(() =>
                {
                    PlayPersistentEffectEvent(persistentEffect.OnEOT);
                });
        }

        private void PlayPersistentEffectEvent(UntargetedGameAction gameAction)
        {
            PlayDialog(gameAction.Message, () =>
            {
                RunUntargetedStatsActions(gameAction.StatsActionProperties);
            });
        }
        #endregion

        #region Stage Combat Events
        private void StageCombatEvents()
        {
            PushEvent(() =>
            {
                PlayDialog(this.CurrentCombatAction.CurrentInfo.GetOpeningMessage(), () =>
                {
                    switch (this.CurrentCombatAction.CombatActionType)
                    {
                        case Enums.CombatActionType.Item:
                            RunTargetedStatsActions(this.CurrentCombatAction.GetStatsActions());
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                });
            });
        }
        #endregion

        private void RunUntargetedStatsActions(IEnumerable<CharacterStatsActionProperties> actions)
        {
            foreach (CharacterStatsActionProperties action in actions)
            {
                StageUntargetedStatsAction(action);
            }

            Continue();
        }

        private void StageUntargetedStatsAction(CharacterStatsActionProperties action)
        {
            PushEvent(() =>
            {
                this.animatorController.ChangeAnimState(Strings.Animation.Flashing, () =>
                {
                    return action.Run(this.Stats, this.Stats);
                });
            });
        }

        private void RunTargetedStatsActions(IEnumerable<CharacterStatsAction> actions)
        {
            foreach (CharacterStatsAction action in actions)
            {
                StageTargetedStatsAction(action);
            }

            Continue();
        }

        private void StageTargetedStatsAction(CharacterStatsAction action)
        {
            PushEvent(() =>
            {
                const string DEFAULT_MESSAGE = "But no one was affected.";

                this.animatorController.ChangeAnimState(Strings.Animation.Attack, () =>
                {
                    string message = DEFAULT_MESSAGE;
                    foreach (CharacterCombat target in action.TargetInfo.CombatTargets)
                    {
                        if (target.Stats.Dead)
                            continue;

                        target.PlayAnimation(action.Properties.GetAnimationKey());

                        if (message == DEFAULT_MESSAGE)
                            message = string.Empty;
                        message += action.Properties.Run(this.stats, target.Stats);
                    }
                    return message;
                });
            });
        }

        private void DeathEvent()
        {
            PlayDeathMessage();
            FlushEvents();
            //End the turn and pass it to the next target
        }

        private void PlayDeathMessage()
        {
            PlayMessage($"{this.Stats.Name} died.");
        }

        private void PlayDialog(List<string> dialog)
        {
            PlayDialog(dialog, null);
        }

        private void PlayDialog(List<string> dialog, Action callback)
        {
            CombatManager.Instance.OrphanedTextbox.Dialog(dialog, 0, () =>
            {
                CombatManager.Instance.OrphanedTextbox.UtilityFinalize();
                callback?.Invoke();
            }, new List<string>
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
            }, new List<string>
            {
                this.Stats.Name
            });
            CombatManager.Instance.OrphanedTextbox.View.gameObject.SetActive(true);
            CombatManager.Instance.OrphanedTextbox.UtilityInitialize();
        }
    }
}
