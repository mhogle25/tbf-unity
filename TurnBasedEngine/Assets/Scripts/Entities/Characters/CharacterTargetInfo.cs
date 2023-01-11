using System.Collections.Generic;
using UnityEngine;
using BF2D.Game;
using BF2D.Combat;
using System;

namespace BF2D.Game
{
    public class CharacterTargetInfo
    {
        public IEnumerable<CharacterCombat> CombatTargets { get { return this.combatTargets; } set { this.combatTargets = value; } }
        private IEnumerable<CharacterCombat> combatTargets = null;

        //Also add overworld information
        //public IEnumerable<CharacterOverworld> OverworldTargets { get { return this.overworldTargets; } set { this.overworldTargets = value; } }
        //private IEnumerable<CharacterOverworld> overworldTargets = null;
    }
}