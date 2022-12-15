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

namespace BF2D.Combat
{
    public class CharacterCombat : MonoBehaviour
    {
        public CharacterType Type { get { return this.stats.Type; } }

        public DialogTextboxControl TextboxControl { set { this.textboxControl = value; } }
        private DialogTextboxControl textboxControl = null;

        public CharacterStats Stats
        {
            get
            {
                return this.stats;
            }
            set
            {
                this.Stats = value;
            }
        }
        private CharacterStats stats;

        public void RunItemAction(ItemCombatAction combatActions)
        {
            throw new NotImplementedException();
        }
    }
}
