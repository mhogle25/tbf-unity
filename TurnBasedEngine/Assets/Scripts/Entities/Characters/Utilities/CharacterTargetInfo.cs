using System.Collections.Generic;
using UnityEngine;
using BF2D.Game;
using System;
using BF2D.Game.Actions;

namespace BF2D.Game
{
    public class CharacterTargetInfo
    {
        public delegate string RunAction(CharacterStats source, CharacterStats target);

        public IEnumerable<Combat.CharacterCombat> CombatTargets { get { return this.combatTargets; } set { this.combatTargets = value; } }
        private IEnumerable<Combat.CharacterCombat> combatTargets = null;

        //Also add overworld information
        //public IEnumerable<CharacterOverworld> OverworldTargets { get { return this.overworldTargets; } set { this.overworldTargets = value; } }
        //private IEnumerable<CharacterOverworld> overworldTargets = null;
    }
}