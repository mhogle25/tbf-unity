using BF2D.UI;
using BF2D.Game.Actions;
using System.Collections.Generic;
using BF2D.Game.Combat.Enums;
using UnityEngine;
using System;

namespace BF2D.Game.Combat.Actions
{
    public class CombatAction
    {
        public CombatActionType Type => this.type;
        private CombatActionType type = CombatActionType.Event;

        public ActCombatActionInfo Act 
        { 
            get => this.Type == CombatActionType.Act ? this.actCombatAction : null; 
            init
            {
                this.type = CombatActionType.Act;
                this.actCombatAction = value;
            } 
        }
        private readonly ActCombatActionInfo actCombatAction = null;

        public EquipCombatActionInfo Equip 
        { 
            get => this.Type == CombatActionType.Equip ? this.equipCombatAction : null; 
            init 
            {
                this.type = CombatActionType.Equip;
                this.equipCombatAction = value; 
            } 
        }
        private readonly EquipCombatActionInfo equipCombatAction = null;

        public EventCombatActionInfo Event { 
            get => this.Type == CombatActionType.Event ? this.eventCombatAction : null; 
            init
            {
                this.type = CombatActionType.Event;
                this.eventCombatAction = value; 
            } 
        }
        private readonly EventCombatActionInfo eventCombatAction = null;

        public FleeCombatActionInfo Flee 
        { 
            get => this.Type == CombatActionType.Flee ? this.fleeCombatAction : null; 
            init
            {
                this.type = CombatActionType.Flee;
                this.fleeCombatAction = value; 
            } 
        }
        private readonly FleeCombatActionInfo fleeCombatAction = null;

        public ItemCombatActionInfo Item 
        { 
            get => this.Type == CombatActionType.Item ? this.itemCombatAction : null; 
            init
            {
                this.type = CombatActionType.Item;
                this.itemCombatAction = value; 
            } 
        }
        private readonly ItemCombatActionInfo itemCombatAction = null;
        
        public RosterCombatActionInfo Roster 
        { 
            get => this.Type == CombatActionType.Roster ? this.rosterCombatAction : null; 
            init 
            {

                this.type = CombatActionType.Roster;
                this.rosterCombatAction = value;
            } 
        }
        private readonly RosterCombatActionInfo rosterCombatAction = null;

        public void SetupControlled()
        {
            switch (this.type)
            {
                case CombatActionType.Act: break;   //TODO
                case CombatActionType.Equip:
                    CombatCtx.One.RunCombatEvents();
                    break;
                case CombatActionType.Event: break; //TODO
                case CombatActionType.Flee: break;  //TODO
                case CombatActionType.Roster: break;
                default: throw new Exception($"[CombatAction:GetStatsAction] {this.type} should be set up with a targeter");
            }
        }

        public void SetupAI(CharacterCombat source)
        {
            switch (this.type)
            {
                case CombatActionType.Act: break;   //TODO
                case CombatActionType.Equip:
                    CombatCtx.One.RunCombatEvents();
                    break;
                case CombatActionType.Event: break; //TODO
                case CombatActionType.Flee: break;  //TODO
                case CombatActionType.Item:
                    source.Stats.CombatAI.SetupTargetedGameAction(GetTargetedGameAction(), source);
                    CombatCtx.One.RunCombatEvents();
                    break;
                case CombatActionType.Roster: break;
            }
        }

        public TargetedGameAction GetTargetedGameAction() => this.type switch
        {
            CombatActionType.Act => null,//TODO
            CombatActionType.Item => this.Item.OnUse,
            _ => throw new Exception("[CombatAction:GetTargetedGems] Tried to get the list of gems but the CombatAction was a type other than Act or Item."),
        };
        

        public TargetedGameAction UseTargetedGameAction() => this.type switch
        {
            CombatActionType.Act => null,//TODO
            CombatActionType.Item => this.Item.UseOnUse(),
            _ => throw new Exception("[CombatAction:GetTargetedGems] Tried to get the list of gems but the CombatAction was a type other than Act or Item."),
        };

        public UntargetedGameAction GetUntargetedGameAction() => this.type switch
        {
            CombatActionType.Equip => this.Equip.OnEquip,
            _ => throw new Exception("[CombatAction:GetGems] Tried to get the list of gems but the CombatAction was a type other than Act or Item."),
        };

        public List<string> Run() => this.type switch
        {
            CombatActionType.Equip => this.Equip.Run(),
            _ => throw new Exception("[CombatAction:GetGems] Tried to get the list of gems but the CombatAction was a type other than Act or Item."),
        };
    }
}
