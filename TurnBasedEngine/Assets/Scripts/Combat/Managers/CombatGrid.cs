using UnityEngine;

namespace BF2D.Combat
{
    public class CombatGrid : MonoBehaviour
    {
        [SerializeField] private CombatGridTile[] playerPlatforms = new CombatGridTile[4];
        [SerializeField] private CombatGridTile[] enemyPlatforms = new CombatGridTile[4];

        public void UpdatePlayerPosition(CharacterCombat characterCombat)
        {

        }
    }
}
