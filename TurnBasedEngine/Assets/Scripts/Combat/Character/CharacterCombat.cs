using UnityEngine;
using BF2D.Game;
using BF2D.Combat.Actions;
using BF2D.UI;
using System.Collections.Generic;
using BF2D.Game.Actions;
using System;

namespace BF2D.Combat
{
    public class CharacterCombat : MonoBehaviour
    {
        private const string IDLE = "idle";
        private const string ATTACK = "attack";
        private const string FLASHING = "flashing";

        [SerializeField] private Animator animator = null;
        private string animState = CharacterCombat.IDLE;
        private delegate string RunEvent();
        private RunEvent animEvent = null;

        public CombatAction CurrentCombatAction { get { return this.currentCombatAction; } }
        private CombatAction currentCombatAction = null;

        public CombatGridTile Tile { set { this.assignedTile = value; } }
        private CombatGridTile assignedTile = null;

        private readonly Queue<CharacterStatsActionProperties> stagedUntargetedStatsActions = new();


        public CharacterStats Stats
        {
            get
            {
                return this.stats;
            }
            set
            {
                this.stats = value;
            }
        }
        private CharacterStats stats;

        #region Public Utilities
        public void SetupCombatAction(CombatAction combatAction)
        {
            this.currentCombatAction = combatAction;
            this.currentCombatAction.Setup();
        }

        public void Destroy()
        {
            if (this.assignedTile)
                this.assignedTile.ResetTile();

            Destroy(this.gameObject);
        }

        public void UpkeepInit()
        {
            CombatManager.Instance.OrphanedTextbox.OnEndOfStackedDialogs.AddListener(() =>
            {
                //CombatInit
                CombatManager.Instance.OrphanedTextbox.OnEndOfStackedDialogs.RemoveAllListeners();
            });

            foreach (StatusEffect statusEffect in this.Stats.StatusEffects)
            {
                CombatManager.Instance.OrphanedTextbox.Dialog(statusEffect.OnUpkeep.Message, 0, () =>
                {
                    CombatManager.Instance.OrphanedTextbox.UtilityFinalize();
                    RunStatusEffect(statusEffect);
                }, new List<string>
                {
                    this.Stats.Name
                });
            }

            CombatManager.Instance.OrphanedTextbox.View.gameObject.SetActive(true);
            CombatManager.Instance.OrphanedTextbox.UtilityInitialize();
        }
        #endregion

        #region Animation Events
        public void AnimTrigger()
        {
            string message = InvokeAnimEvent();
            CombatManager.Instance.OrphanedTextbox.Message(message, () => 
            {
                CombatManager.Instance.OrphanedTextbox.UtilityFinalize();
                PlayStatsActionAnimation();
            });
            CombatManager.Instance.OrphanedTextbox.UtilityInitialize();
        }

        public void AnimSwitchIdle()
        {
            ChangeAnimState(CharacterCombat.IDLE);
        }
        #endregion

        private void RunStatusEffect(StatusEffect statusEffect)
        {
            if (statusEffect.Duration > 0)
                statusEffect.Use();
            if (statusEffect.Duration == 0)
            {
                this.Stats.StatusEffects.Remove(statusEffect);
            }
            RunUntargetedStatsActions(statusEffect.OnUpkeep.StatsActionProperties);
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
            //PLAY ANIMATION
            //Animation triggers animEvent which runs the action and gives the message it creates to the orphaned textbox
            ChangeAnimState(CharacterCombat.FLASHING, () =>
            {
                return action.Run(this.Stats, this.Stats);
            });
        }

        private void ChangeAnimState(string newState)
        {
            ChangeAnimState(newState, null);
        }

        private void ChangeAnimState(string newState, RunEvent callback)
        {
            if (this.animState == newState) return;
            this.animator.Play(newState);
            this.animState = newState;
            this.animEvent = callback;
        }

        private string InvokeAnimEvent()
        {
            string message = this.animEvent?.Invoke();
            this.animEvent = null;
            return message;
        }
    }
}
