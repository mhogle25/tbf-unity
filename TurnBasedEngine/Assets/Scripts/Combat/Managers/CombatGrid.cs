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
        [Header("Platforms")]
        [SerializeField] private CombatGridTile[] playerPlatforms = new CombatGridTile[Macros.MaxPartySize];
        [SerializeField] private CombatGridTile[] enemyPlatforms = new CombatGridTile[Macros.MaxPartySize];
        [Header("Prefabs")]
        [SerializeField] private List<CharacterCombat> playerCombatPrefabs = new();
        [SerializeField] private List<CharacterCombat> enemyCombatPrefabs = new();

        private readonly Dictionary<string, CharacterCombat> playerCombatPrefabsDict = new();
        private readonly Dictionary<string, CharacterCombat> enemyCombatPrefabsDict = new();

        private readonly List<CharacterCombat> characterQueue = new();
        private readonly List<CharacterStats> defeatedEnemies = new();

        public int PlayerCount { get { return this.playersCount; } }
        private int playersCount = 0;
        public int EnemyCount { get { return this.enemiesCount; } }
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
        public int GetTotalExperience()
        {
            if (CombatManager.Instance.CombatIsOver())
            {
                Terminal.IO.LogWarning("[CombatGrid:GetTotalExperience] Tried to get the total experience from the fight but combat wasn't over.");
                return 0;
            }

            int value = 0;
            foreach (CharacterStats character in this.defeatedEnemies)
                value += character.CurrentJob.ExperienceAward;
            return value;
        }

        public void Setup(List<CharacterStats> players, List<CharacterStats> enemies)
        {
            if (!DictsLoaded())
                LoadCharacterPrefabs();

            players.Sort((x, y) => x.GridPosition.CompareTo(y.GridPosition));
            enemies.Sort((x, y) => x.GridPosition.CompareTo(y.GridPosition));

            foreach (CharacterStats playerStats in players)
            {
                if (IsPlayerGridFull())
                {
                    Terminal.IO.LogWarning("[CombatGrid:Setup] Tried to instantiate a player combat but the grid was full");
                    continue;
                }
                CharacterCombat playerCombat = InstantiatePlayerCombat(playerStats);
                this.playerPlatforms[this.playersCount].AssignCharacter(playerCombat);
                this.characterQueue.Add(playerCombat);
                this.playersCount++;
            }

            foreach (CharacterStats enemyStats in enemies)
            {
                if (IsEnemyGridFull())
                {
                    Terminal.IO.LogWarning("[CombatGrid:Setup] Tried to instantiate an enemy combat but the grid was full");
                    continue;
                }
                CharacterCombat enemyCombat = InstantiateEnemyCombat(enemyStats);
                this.enemyPlatforms[this.enemiesCount].AssignCharacter(enemyCombat);
                this.characterQueue.Add(enemyCombat);
                this.enemiesCount++;
            }

            SpeedSort();

        }

        public void PassTurn()
        {
            List<CharacterCombat> toBeDestroyed = new();

            this.characterQueue.RemoveAll((character) =>
            {
                if (character.Stats.Dead)
                {
                    if (CharacterIsEnemy(character))
                    {
                        this.defeatedEnemies.Add(character.Stats);
                        this.enemiesCount--;
                    }

                    if (CharacterIsPlayer(character))
                    {
                        this.playersCount--;
                    }
                    toBeDestroyed.Add(character);
                }

                return character.Stats.Dead;
            });

            foreach (CharacterCombat character in toBeDestroyed)
                character.Destroy();

            this.currentCharacterIndex++;

            if (this.currentCharacterIndex >= this.characterQueue.Count)
            {
                SpeedSort();
                this.currentCharacterIndex = 0;
            }
        }

        public bool CharacterIsPlayer(CharacterCombat character)
        {
            return this.Players.Contains(character);
        }

        public bool CharacterIsEnemy(CharacterCombat character)
        {
            return this.Enemies.Contains(character);
        }

        public void GridReset()
        {
            foreach (CombatGridTile tile in this.playerPlatforms)
            {
                if (tile.AssignedCharacter != null)
                    tile.AssignedCharacter.Destroy();
            }

            foreach (CombatGridTile tile in this.enemyPlatforms)
            {
                if (tile.AssignedCharacter != null)
                    tile.AssignedCharacter.Destroy();
            }

            this.characterQueue.Clear();
            this.defeatedEnemies.Clear();
            this.currentCharacterIndex = 0;
            this.playersCount = 0;
            this.enemiesCount = 0;
        }
        #endregion

        #region Private Methods
        private void SpeedSort()
        {
            this.characterQueue.Sort((x, y) => y.Stats.Speed.CompareTo(x.Stats.Speed));
        }

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
            CharacterCombat player = Instantiate(this.playerCombatPrefabsDict[playerStats.PrefabID]);
            player.Stats = playerStats;
            return player;
        }

        private CharacterCombat InstantiateEnemyCombat(CharacterStats enemyStats)
        {
            CharacterCombat player = Instantiate(this.enemyCombatPrefabsDict[enemyStats.PrefabID]);
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
