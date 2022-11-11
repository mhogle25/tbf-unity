using UnityEngine;

namespace BF2D.Combat
{
    public class CombatGrid : MonoBehaviour
    {
        [SerializeField] private CombatGridTile[] playerPlatforms = new CombatGridTile[4];
        [SerializeField] private CombatGridTile[] enemyPlatforms = new CombatGridTile[4];

        public void UpdatePlayerPosition(PlayerCombat playerCombat, int gridPosition)
        {
            if (!GridPositionValid(gridPosition, this.playerPlatforms.Length))
            {
                Debug.LogError($"[CombatGrid] Tried to initialize PlayerCombat '{playerCombat.name}' but could not set the position");
                return;
            }

            PositionPlayer(playerCombat, gridPosition);
        }

        public void UpdateEnemyPosition(EnemyCombat enemyCombat, int gridPosition)
        {
            if (!GridPositionValid(gridPosition, this.enemyPlatforms.Length))
            {
                Debug.LogError($"[CombatGrid] Tried to initialize EnemyCombat '{enemyCombat.name}' but could not set the position");
                return;
            }

            PositionEnemy(enemyCombat, gridPosition);
        }

        private bool GridPositionValid(int gridPosition, int playersLength)
        {
            if (gridPosition > playersLength - 1 || gridPosition < 0)
            {
                Debug.LogError($"[CombatGrid] The specified grid position {gridPosition} was out of range (0, {playersLength - 1})");
                return false;
            }

            return true;
        }

        private void PositionPlayer(PlayerCombat playerCombat, int newPosition)
        {
            this.enemyPlatforms[playerCombat.Stats.GridPosition].ResetTile();
            playerCombat.Stats.GridPosition = newPosition;
            this.playerPlatforms[playerCombat.Stats.GridPosition].AssignCharacter(playerCombat);
        }

        private void PositionEnemy(EnemyCombat enemyCombat, int newPosition)
        {
            this.enemyPlatforms[enemyCombat.Stats.GridPosition].ResetTile();
            enemyCombat.Stats.GridPosition = newPosition;
            this.enemyPlatforms[enemyCombat.Stats.GridPosition].AssignCharacter(enemyCombat);
        }
    }
}
