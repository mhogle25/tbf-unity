using BF2D.Game.Actions;
using System.Collections.Generic;
using UnityEngine;
using BF2D.Game;

namespace BF2D.Combat.Actions
{
    public class TargetedStatsAction
    {
        public CharacterStatsAction StatsAction { get { return this.statsAction; } set { this.statsAction = value; } }
        private CharacterStatsAction statsAction = null;
        public IEnumerable<CharacterCombat> Targets { get { return this.targets; } set { this.targets = value; } }
        private IEnumerable<CharacterCombat> targets = null;
    }
}