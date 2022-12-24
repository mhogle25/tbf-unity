using BF2D.Enums;
using BF2D.UI;
using BF2D.Game.Actions;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D.Combat.Actions
{
    public class CombatAction
    {
        public CombatActionType CombatActionType { get { return this.combatActionType; } set { this.combatActionType = value; } }
        private CombatActionType combatActionType = CombatActionType.Event;
        public ActCombatActionInfo Act { get { return this.actCombatAction; } set { this.actCombatAction = value; } }
        private ActCombatActionInfo actCombatAction = null;
        public EquipCombatActionInfo Equip { get { return this.equipCombatAction; } set { this.equipCombatAction = value; } }
        private EquipCombatActionInfo equipCombatAction = null;
        public EventCombatActionInfo Event { get { return this.eventCombatAction; } set { this.eventCombatAction = value; } }
        private EventCombatActionInfo eventCombatAction = null;
        public FleeCombatActionInfo Flee { get { return this.fleeCombatAction; } set { this.fleeCombatAction = value; } }
        private FleeCombatActionInfo fleeCombatAction = null;
        public ItemCombatActionInfo Item { get { return this.itemCombatAction; } set { this.itemCombatAction = value; } }
        private ItemCombatActionInfo itemCombatAction = null;
        public RosterCombatActionInfo Roster { get { return this.rosterCombatAction; } set { this.rosterCombatAction = value; } }
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

        public void SetTargetedStatsActions(Queue<TargetedStatsAction> targetedActions)
        {
            switch (this.combatActionType)
            {
                case CombatActionType.Act: return; //TODO
                case CombatActionType.Item: this.itemCombatAction.TargetedActions = targetedActions; return;
                default: Debug.LogError("[CombatAction:SetTargetedStatsAction] Tried to set the list of TargetedStatsActions but the CombatAction was a type other than Act or Item."); return;
            }
        }
    }
}
