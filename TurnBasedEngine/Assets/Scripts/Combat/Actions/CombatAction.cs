using BF2D.UI;
using BF2D.Game.Actions;
using System.Collections.Generic;
using BF2D.Game.Combat.Enums;

namespace BF2D.Game.Combat.Actions
{
    public class CombatAction
    {
        public CombatActionType Type => this.type;
        private CombatActionType type = CombatActionType.Event;
        /// <summary>
        /// Resets all other combat action types referenced in this object
        /// </summary>
        public ActCombatActionInfo Act 
        { 
            get 
            { 
                return this.Type == CombatActionType.Act ? this.actCombatAction : null; 
            } 
            set 
            {
                this.type = CombatActionType.Act;
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
                return this.Type == CombatActionType.Equip ? this.equipCombatAction : null; 
            } 
            set 
            {
                this.type = CombatActionType.Equip;
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
                return this.Type == CombatActionType.Event ? this.eventCombatAction : null; 
            } 
            set
            {
                this.type = CombatActionType.Event;
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
                return this.Type == CombatActionType.Flee ? this.fleeCombatAction : null; 
            } 
            set
            {
                this.type = CombatActionType.Flee;
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
                return this.Type == CombatActionType.Item ? this.itemCombatAction : null; 
            } 
            set
            {
                this.type = CombatActionType.Item;
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
                return this.Type == CombatActionType.Roster ? this.rosterCombatAction : null; 
            } 
            set 
            {

                this.type = CombatActionType.Roster;
                this.rosterCombatAction = value;
            } 
        }
        private RosterCombatActionInfo rosterCombatAction = null;

        public ICombatActionInfo CurrentInfo
        {
            get
            {
                return this.type switch
                {
                    CombatActionType.Flee => this.Flee,
                    CombatActionType.Act => this.Act,
                    CombatActionType.Item => this.Item,
                    CombatActionType.Roster => this.Roster,
                    CombatActionType.Equip => this.Equip,
                    CombatActionType.Event => this.Event,
                    _ => null,
                };
            }
        }

        public void SetupControlled(UIControl targeter)
        {
            switch (this.type)
            {
                case CombatActionType.Act: break;   //TODO
                case CombatActionType.Equip: break; //TODO
                case CombatActionType.Event: break; //TODO
                case CombatActionType.Flee: break;  //TODO
                case CombatActionType.Item:
                    if (this.Item.HasGems)
                        UIControlsManager.Instance.TakeControl(targeter);
                    //TODO
                    break;
                case CombatActionType.Roster: break;
            }
        }

        public void SetupAI(CharacterCombatAI ai)
        {
            switch (this.type)
            {
                case CombatActionType.Act: break;   //TODO
                case CombatActionType.Equip: break; //TODO
                case CombatActionType.Event: break; //TODO
                case CombatActionType.Flee: break;  //TODO
                case CombatActionType.Item:
                    if (this.Item.HasGems)
                        ai.SetupTargetedGems();
                    //TODO
                    break;
                case CombatActionType.Roster: break;
            }
        }

        public IEnumerable<TargetedCharacterStatsAction> GetTargetedGems()
        {
            switch (this.type)
            {
                case CombatActionType.Act: return null; //TODO
                case CombatActionType.Item: return this.Item.Gems;
                default: Terminal.IO.LogError("[CombatAction:GetStatsAction] Tried to get the list of CharacterStatsActions but the CombatAction was a type other than Act or Item."); return null;
            }
        }
        

        public IEnumerable<TargetedCharacterStatsAction> UseTargetedGems()
        {
            switch (this.type)
            {
                case CombatActionType.Act: return null; //TODO
                case CombatActionType.Item: return this.Item.UseGems();
                default: Terminal.IO.LogError("[CombatAction:GetStatsAction] Tried to get the list of CharacterStatsActions but the CombatAction was a type other than Act or Item."); return null;
            }
        }
    }
}
