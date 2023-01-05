using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using BF2D.Game;
using UnityEngine.TextCore.Text;

namespace BF2D.Combat
{
    public class CombatGrid : MonoBehaviour
    {
        [SerializeField] private CombatGridTile[] playerPlatforms = new CombatGridTile[4];
        [SerializeField] private CombatGridTile[] enemyPlatforms = new CombatGridTile[4];
        [Header("Prefabs")]
        [SerializeField] private List<CharacterCombat> playerCombatPrefabs = new();
        [SerializeField] private List<CharacterCombat> enemyCombatPrefabs = new();

        private readonly Dictionary<string, CharacterCombat> playerCombatPrefabsDict = new();
        private readonly Dictionary<string, CharacterCombat> enemyCombatPrefabsDict = new();

        private readonly List<CharacterCombat> characterQueue = new();

        private int playersCount = 0;
        private int enemiesCount = 0;

        private int currentCharacterIndex = 0;

        #region Getter Setters
        public IEnumerable<CharacterCombat> Players
        {
            get
            {
                List<CharacterCombat> list = new();
                foreach (CombatGridTile tile in this.playerPlatforms)
                {
                    if (tile.AssignedCharacter)
                        list.Add(tile.AssignedCharacter);
                }
                return list;
            }
        }
        public IEnumerable<CharacterCombat> Enemies
        {
            get
            {
                List<CharacterCombat> list = new();
                foreach (CombatGridTile tile in this.enemyPlatforms)
                {
                    if (tile.AssignedCharacter)
                        list.Add(tile.AssignedCharacter);
                }
                return list;
            }
        }
        public IEnumerable<CharacterCombat> Characters
        {
            get
            {
                return this.characterQueue;
            }
        }
        public CharacterCombat CurrentCharacter
        {
            get
            {
                return this.characterQueue[this.currentCharacterIndex]; 
            }
        }
        #endregion

        #region Public Utilities
        public void DestroyCharacter(CharacterCombat character)
        {
            this.characterQueue.Remove(character);
            character.Destroy();
        }

        public void EndTurn()
        {
            this.currentCharacterIndex++;
        }

        public void Setup(List<CharacterStats> players, List<CharacterStats> enemies)
        {
            if (!DictsLoaded())
                LoadCharacterPrefabs();

            players.Sort((x, y) => x.GridPosition.CompareTo(y.GridPosition));
            enemies.Sort((x, y) => x.GridPosition.CompareTo(y.GridPosition));

            foreach (CharacterStats playerStats in players)
            {
                if (IsEnemyGridFull())
                {
                    Debug.LogWarning("[CombatGrid:Setup] Tried to instantiate an EnemyCombat but the grid was full");
                    return;
                }
                CharacterCombat playerCombat = InstantiatePlayerCombat(playerStats);
                this.playerPlatforms[this.playersCount].AssignCharacter(playerCombat);
                this.characterQueue.Add(playerCombat);
                this.playersCount++;
            }

            foreach (CharacterStats enemyStats in enemies)
            {
                if (IsPlayerGridFull())
                {
                    Debug.LogWarning("[CombatGrid:Setup] Tried to instantiate a PlayerCombat but the grid was full");
                    return;
                }
                CharacterCombat enemyCombat = InstantiateEnemyCombat(enemyStats);
                this.enemyPlatforms[this.enemiesCount].AssignCharacter(enemyCombat);
                this.characterQueue.Add(enemyCombat);
                this.enemiesCount++;
            }

            this.characterQueue.Sort((x, y) => x.Stats.Speed.CompareTo(y.Stats.Speed));
        }
        #endregion

        #region Private Methods
        private void LoadCharacterPrefabs()
        {
            foreach (CharacterCombat combatPrefab in this.playerCombatPrefabs)
            {
                this.playerCombatPrefabsDict.Add(combatPrefab.name, combatPrefab);
            }

            foreach (CharacterCombat combatPrefab in this.enemyCombatPrefabs)
            {
                this.enemyCombatPrefabsDict.Add(combatPrefab.name, combatPrefab);
            }
        }
        
        private CharacterCombat InstantiatePlayerCombat(CharacterStats playerStats)
        {
            CharacterCombat player = Instantiate(this.playerCombatPrefabsDict[playerStats.ID]);
            player.Stats = playerStats;
            return player;
        }

        private CharacterCombat InstantiateEnemyCombat(CharacterStats enemyStats)
        {
            CharacterCombat player = Instantiate(this.enemyCombatPrefabsDict[enemyStats.ID]);
            player.Stats = enemyStats;
            return player;
        }

        private bool IsPlayerGridFull()
        {
            return this.playersCount >= this.playerPlatforms.Length;
        }

        private bool IsEnemyGridFull()
        {
            return this.enemiesCount >= this.enemyPlatforms.Length;
        }

        private bool DictsLoaded()
        {
            return this.playerCombatPrefabsDict.Count > 0 && this.enemyCombatPrefabsDict.Count > 0;
        }
        #endregion
    }
}