using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BF2D.Game;
using BF2D.Game.Actions;
using BF2D.Enums;
using System;

namespace BF2D.Combat
{
    public class PlayerCombat : CharacterCombat
    {
        public override CharacterType Type { get { return CharacterType.Player; } }

        public PlayerStats Stats { get { return this.stats; } set { this.stats = value; } }
        private PlayerStats stats = null;

        public override void TriggerGameAction()
        {
            TriggerGameAction(this.stats);
        }
    }
}
