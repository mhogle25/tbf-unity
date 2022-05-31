using UnityEngine;
using BF2D.Game;

namespace BF2D.Combat
{

    public class EnemyCombat : CharacterCombat
    {
        public override Stats GetStats { get { return this.enemyStats; } }
        public EnemyStats GetEnemyStats { get { return this.enemyStats; } }
        private EnemyStats enemyStats = null;
    }
}
