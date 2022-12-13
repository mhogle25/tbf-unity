using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BF2D.Game;

namespace BF2D.Combat
{
    public class ItemCombatAction
    {
        public Item Item { get { return this.item; } set { this.item = value; } }
        private Item item = null;
        public Queue<CombatAction> CombatActions { get { return this.combatActions; } set { this.combatActions = value; } }
        private Queue<CombatAction> combatActions = new();
    }
}
