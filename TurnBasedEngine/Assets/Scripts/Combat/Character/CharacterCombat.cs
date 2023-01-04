using UnityEngine;
using BF2D.Game;
using BF2D.Game.Actions;
using BF2D.Enums;
using System;
using System.IO;
using BF2D.UI;
using System.Collections.Generic;
using UnityEditor;
using Newtonsoft.Json.Linq;
using BF2D.Combat.Actions;

namespace BF2D.Combat
{
    public class CharacterCombat : MonoBehaviour
    {
        //public CharacterType Type { get { return this.stats.Type; } }

        public CombatAction CurrentCombatAction { get { return this.combatAction; } }
        private CombatAction combatAction = null;

        public CombatGridTile Tile { set { this.assignedTile = value; } }
        private CombatGridTile assignedTile = null;

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

        public void SetupCombatAction(CombatAction combatAction)
        {
            this.combatAction = combatAction;
            this.combatAction.Setup();
        }

        public void Destroy()
        {
            this.assignedTile.ResetTile();
            Destroy(this.gameObject);
        }
    }
}
