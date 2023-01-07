using BF2D.Enums;
using BF2D.UI;
using BF2D.Game.Actions;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D.Combat.Actions
{
    public class CombatAction
    {
        public CombatActionType CombatActionType { get { return this.combatActionType; } }
        private CombatActionType combatActionType = CombatActionType.Event;
        /// <summary>
        /// Resets all other combat action types referenced in this object
        /// </summary>
        public ActCombatActionInfo Act 
        { 
            get 
            { 
                return this.actCombatAction; 
            } 
            set 
            {
                this.combatActionType = CombatActionType.Act;
                ResetInfos();
                this.actCombatAction = value;
            } 
        }
        private ActCombatActionInfo actCombatAction = null;
        /// <summary>
        /// Resets all other combat action types referenced in this object
        /// </summary>
        public EquipCombatActionInfo Equip 
        { 
            get 
            { 
                return this.equipCombatAction; 
            } 
            set 
            {
                this.combatActionType = CombatActionType.Equip;
                ResetInfos();
                this.equipCombatAction = value; 
            } 
        }
        private EquipCombatActionInfo equipCombatAction = null;
        /// <summary>
        /// Resets all other combat action types referenced in this object
        /// </summary>
        public EventCombatActionInfo Event { 
            get 
            { 
                return this.eventCombatAction; 
            } 
            set
            {
                this.combatActionType = CombatActionType.Event;
                ResetInfos();
                this.eventCombatAction = value; 
            } 
        }
        private EventCombatActionInfo eventCombatAction = null;
        /// <summary>
        /// Resets all other combat action types referenced in this object
        /// </summary>
        public FleeCombatActionInfo Flee 
        { 
            get 
            { 
                return this.fleeCombatAction; 
            } 
            set
            {
                this.combatActionType = CombatActionType.Flee;
                ResetInfos();
                this.fleeCombatAction = value; 
            } 
        }
        private FleeCombatActionInfo fleeCombatAction = null;
        /// <summary>
        /// Resets all other combat action types referenced in this object
        /// </summary>
        public ItemCombatActionInfo Item 
        { 
            get 
            { 
                return this.itemCombatAction; 
            } 
            set
            {
                this.combatActionType = CombatActionType.Item;
                ResetInfos();
                this.itemCombatAction = value; 
            } 
        }
        private ItemCombatActionInfo itemCombatAction = null;
        /// <summary>
        /// Resets all other combat action types referenced in this object
        /// </summary>
        public RosterCombatActionInfo Roster 
        { 
            get 
            { 
                return this.rosterCombatAction; 
            } 
            set 
            {

                this.combatActionType = CombatActionType.Roster;
                ResetInfos();
                this.rosterCombatAction = value; 
            } 
        }
        private RosterCombatActionInfo rosterCombatAction = null;

        public void Setup()
        {
            switch (this.combatActionType)
            {
                case CombatActionType.Act: break;   //TODO
                case CombatActionType.Equip: break; //TODO
                case CombatActionType.Event: break; //TODO
                case CombatActionType.Flee: break;  //TODO
                case CombatActionType.Item:
                    UIControlsManager.Instance.TakeControl(CombatManager.Instance.CharacterTargeter);
                    break;
                case CombatActionType.Roster: break;
            }
        }

        public List<CharacterStatsAction> GetStatsActions()
        {
            switch (this.combatActionType)
            {
                case CombatActionType.Act: return null; //TODO
                case CombatActionType.Item: return this.itemCombatAction.Info.Get().OnUse.StatsActions;
                default: Debug.LogError("[CombatAction:GetStatsAction] Tried to get the list of CharacterStatsActions but the CombatAction was a type other than Act or Item."); return null;
            }
        }

        public void EnqueueTargetedStatsActions(TargetedStatsAction targetedActions)
        {
            switch (this.combatActionType)
            {
                case CombatActionType.Act: return; //TODO
                case CombatActionType.Item: this.itemCombatAction.TargetedActions.Enqueue(targetedActions); return;
                default: Debug.LogError("[CombatAction:SetTargetedStatsAction] Tried to set the list of TargetedStatsActions but the CombatAction was a type other than Act or Item."); return;
            }
        }

        private void ResetInfos()
        {
            this.actCombatAction = null;
            this.equipCombatAction = null;
            this.eventCombatAction = null;
            this.fleeCombatAction = null;
            this.itemCombatAction = null;
            this.rosterCombatAction = null;
        }
    }
}
