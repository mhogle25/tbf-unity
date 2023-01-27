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
            public const string IDLE = "idle";
            public const string ATTACK = "attack";
            public const string FLASHING = "flashing";

            private string animState = AnimatorController.IDLE;
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
                string message = this.animEvent?.Invoke();
                this.animEvent = null;
                return message;
            }
        }

        private AnimatorController animatorController = null;

        public CombatAction CurrentCombatAction { get { return this.currentCombatAction; } }
        private CombatAction currentCombatAction = null;

        public CombatGridTile Tile { set { this.assignedTile = value; } }
        private CombatGridTile assignedTile = null;

        private readonly Queue<CharacterStatsActionProperties> stagedUntargetedStatsActions = new();

        public CharacterStats Stats { get { return this.stats; } set { this.stats = value; } }
        private CharacterStats stats;

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

        public void RunCombat()
        {
            //End Of Turn Cleanup Event
            CombatManager.Instance.OrphanedTextbox.OnEndOfStackedDialogs.AddListener(() =>
            {
                Debug.Log("This event will clean up the game based on the information delta of the turn and pass the turn to the next character");
                CombatManager.Instance.OrphanedTextbox.OnEndOfStackedDialogs.RemoveAllListeners();
            });

            //Stage End Of Turn Events
            StagePersistentEffectEvent(StagePersistentEffectEOTEvent);

            //Stage Combat Events
            CombatManager.Instance.OrphanedTextbox.Message("This should be an introductory message to whatever combat action is currently staged, but I haven't gotten to that functionality yet.", () =>
            {
                Debug.Log("This is when the action is triggered");
            });

            //Stage Upkeep Events
            StagePersistentEffectEvent(StagePersistentEffectUpkeepEvent);

            //Run Staged Events
            CombatManager.Instance.OrphanedTextbox.View.gameObject.SetActive(true);
            CombatManager.Instance.OrphanedTextbox.UtilityInitialize();
        }

        public void Destroy()
        {
            if (this.assignedTile)
                this.assignedTile.ResetTile();

            Destroy(this.gameObject);
        }
        #endregion

        #region Animation Events
        public void AnimTrigger()
        {
            string message = this.animatorController.InvokeAnimEvent();
            CombatManager.Instance.OrphanedTextbox.Message(message, () => 
            {
                CombatManager.Instance.OrphanedTextbox.UtilityFinalize();
                PlayStatsActionAnimation();
            });
            CombatManager.Instance.OrphanedTextbox.UtilityInitialize();
        }

        public void AnimSwitchIdle()
        {
            this.animatorController.ChangeAnimState(AnimatorController.IDLE);
        }
        #endregion

        private void StagePersistentEffectEvent(Action<PersistentEffect> stagingAction)
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
                StagePersistentEffectEventDialog(persistentEffect.OnUpkeep);
        }

        private void StagePersistentEffectEOTEvent(PersistentEffect persistentEffect)
        {
            if (persistentEffect.EOTEventExists())
                StagePersistentEffectEventDialog(persistentEffect.OnEOT);
        }

        private void StagePersistentEffectEventDialog(UntargetedGameAction gameAction)
        {
            CombatManager.Instance.OrphanedTextbox.Dialog(gameAction.Message, 0, () =>
            {
                CombatManager.Instance.OrphanedTextbox.UtilityFinalize();

                RunUntargetedStatsActions(gameAction.StatsActionProperties);
                //Add possible actions other than StatsActionProperties
            }, new List<string>
            {
                this.Stats.Name
            });
        }

        private void RunUntargetedStatsActions(IEnumerable<CharacterStatsActionProperties> actions)
        {
            foreach (CharacterStatsActionProperties action in actions)
            {
                this.stagedUntargetedStatsActions.Enqueue(action);
            }

            PlayStatsActionAnimation();
        }

        private void PlayStatsActionAnimation()
        {
            if (this.stagedUntargetedStatsActions.Count < 1)
            {
                CombatManager.Instance.OrphanedTextbox.UtilityInitialize();
                return;
            }

            CharacterStatsActionProperties action = this.stagedUntargetedStatsActions.Dequeue();

            //Play Animation
            //Animation triggers animEvent which runs the action and gives the message it creates to the orphaned textbox
            this.animatorController.ChangeAnimState(AnimatorController.FLASHING, () =>
            {
                return action.MessageRun(this.Stats, this.Stats);
            });
        }
    }
}
