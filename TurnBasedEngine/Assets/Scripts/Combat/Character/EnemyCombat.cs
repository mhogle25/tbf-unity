using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BF2D.Game;
using BF2D.Game.Actions;
using BF2D.Enums;
using System;

namespace BF2D.Combat
{
    public class EnemyCombat : CharacterCombat
    {
        public override CharacterType Type { get { return CharacterType.Enemy; } }

        public EnemyStats Stats { get { return this.stats; } set { this.stats = value; } }
        private EnemyStats stats = null;

        public override void TriggerGameAction()
        {
            if (this.state.gameAction is null)
                return;

            foreach (CharacterStatsAction statsAction in this.state.gameAction.StatsActions)
            {
                this.state.dialog = statsAction.Run(this.stats);
            }
        }
    }
}
