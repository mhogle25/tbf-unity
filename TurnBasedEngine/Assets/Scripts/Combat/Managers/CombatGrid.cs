using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BF2D.Game.Combat
{
    public class CombatGrid : MonoBehaviour
    {
        [Header("Platforms")]
        [SerializeField] private CombatGridTile[] playerPlatforms = new CombatGridTile[Numbers.MaxPartySize];
        [SerializeField] private CombatGridTile[] enemyPlatforms = new CombatGridTile[Numbers.MaxPartySize];
        [Header("Prefabs")]
        [SerializeField] private List<CharacterCombat> playerCombatPrefabs = new();
        [SerializeField] private List<CharacterCombat> enemyCombatPrefabs = new();

        private readonly Dictionary<string, CharacterCombat> playerCombatPrefabsDict = new();
        private readonly Dictionary<string, CharacterCombat> enemyCombatPrefabsDict = new();

        private readonly List<CharacterCombat> characterQueue = new();
        private readonly List<CharacterStats> defeatedEnemies = new();
        private readonly List<CharacterStats> defeatedPlayers = new();

        private int playersCount = 0;
        private int enemiesCount = 0;

        private int currentCharacterIndex = 0;

        #region Getter Setters
        public CharacterCombat[] ActivePlayers => CreateCharacterList(this.playerPlatforms, tile => !tile.AssignedCharacter.Stats.Dead).ToArray();
        public CharacterCombat[] ActiveEnemies => CreateCharacterList(this.enemyPlatforms, tile => !tile.AssignedCharacter.Stats.Dead).ToArray();
        public CharacterCombat[] ActiveCharacters
        {
            get
            {
                List<CharacterCombat> list = new();
                FillCharacterList(this.playerPlatforms, tile => !tile.AssignedCharacter.Stats.Dead, list).ToArray();
                FillCharacterList(this.enemyPlatforms, tile => !tile.AssignedCharacter.Stats.Dead, list).ToArray();
                return list.ToArray();
            }
        }

        private CharacterCombat[] AllPlayers => CreateCharacterList(this.playerPlatforms, tile => true).ToArray();
        private CharacterCombat[] AllEnemies => CreateCharacterList(this.enemyPlatforms, tile => true).ToArray();
        private CharacterCombat[] AllCharacters
        {
            get
            {
                List<CharacterCombat> list = new();
                list = FillCharacterList(this.playerPlatforms, tile => true, list);
                list = FillCharacterList(this.enemyPlatforms, tile => true, list);
                return list.ToArray();
            }
        }

        public CharacterCombat CurrentCharacter => this.characterQueue[this.currentCharacterIndex];
        #endregion

        #region Public Utilities
        public long GetTotalExperience()
        {
            if (!CombatManager.Instance.CombatIsOver())
            {
                Debug.LogWarning("[CombatGrid:GetTotalExperience] Tried to get the total experience from the fight but combat wasn't over.");
                return 0;
            }

            long value = 0;
            foreach (CharacterStats character in this.defeatedEnemies)
                value += character.CurrentJob.ExperienceAward;
            
            return value;
        }

        public int GetTotalCurrencyLoot()
        {
            if (!CombatManager.Instance.CombatIsOver())
            {
                Debug.LogWarning($"[CombatGrid:GetTotalCurrencyLoot] Tried to get the total {Strings.Game.Currency} loot from the fight but combat wasn't over.");
                return 0;
            }

            int value = 0;
            foreach (CharacterStats character in this.defeatedEnemies)
                value += character.CurrencyLoot;
            return value;
        }

        public int GetTotalEtherLoot()
        {
            if (!CombatManager.Instance.CombatIsOver())
            {
                Debug.LogWarning($"[CombatGrid:GetTotalEtherLoot] Tried to get the total {Strings.Game.Ether} loot from the fight but combat wasn't over.");
                return 0;
            }

            int value = 0;
            foreach (CharacterStats character in this.defeatedEnemies)
                value += character.EtherLoot;
            return value;
        }

        public IEnumerable<string> GetTotalItemLoot()
        {
            if (!CombatManager.Instance.CombatIsOver())
            {
                Debug.LogWarning($"[CombatGrid:GetTotalItemsLoot] Tried to get the total item loot from the fight but combat wasn't over.");
                return new List<string>();
            }

            return GetTotalLoot(character => character.ItemLoot);
        }

        public IEnumerable<string> GetTotalEquipmentLoot()
        {
            if (!CombatManager.Instance.CombatIsOver())
            {
                Debug.LogWarning($"[CombatGrid:GetTotalEquipmentsLoot] Tried to get the total equipment loot from the fight but combat wasn't over.");
                return new List<string>();
            }

            return GetTotalLoot(character => character.EquipmentLoot);
        }

        public IEnumerable<string> GetTotalGemLoot()
        {
            if (!CombatManager.Instance.CombatIsOver())
            {
                Debug.LogWarning($"[CombatGrid:GetTotalGemsLoot] Tried to get the total gem loot from the fight but combat wasn't over.");
                return new List<string>();
            }

            return GetTotalLoot(character => character.GemLoot);
        }

        public void Setup(IEnumerable<CharacterStats> players, IEnumerable<CharacterStats> enemies)
        {
            List<CharacterStats> playersOrdered = new(players);
            List<CharacterStats> enemiesOrdered = new(enemies);

            this.defeatedEnemies.Clear();

            if (!DictsLoaded())
                LoadCharacterPrefabs();

            playersOrdered.Sort((x, y) => x.GridPosition.CompareTo(y.GridPosition));
            enemiesOrdered.Sort((x, y) => x.GridPosition.CompareTo(y.GridPosition));

            foreach (CharacterStats playerStats in playersOrdered)
            {
                if (IsPlayerGridFull())
                {
                    Debug.LogWarning("[CombatGrid:Setup] Tried to instantiate a player combat but the grid was full");
                    continue;
                }
                CharacterCombat playerCombat = InstantiatePlayerCombat(playerStats);
                this.playerPlatforms[this.playersCount].AssignCharacter(playerCombat);
                this.characterQueue.Add(playerCombat);
                this.playersCount++;
            }

            foreach (CharacterStats enemyStats in enemiesOrdered)
            {
                if (IsEnemyGridFull())
                {
                    Debug.LogWarning("[CombatGrid:Setup] Tried to instantiate an enemy combat but the grid was full");
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
                        this.defeatedPlayers.Add(character.Stats);
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
            return this.AllPlayers.Contains(character);
        }

        public bool CharacterIsEnemy(CharacterCombat character)
        {
            return this.AllEnemies.Contains(character);
        }

        public void GridReset()
        {
            foreach (CombatGridTile tile in this.playerPlatforms)
                if (tile.AssignedCharacter != null)
                    tile.AssignedCharacter.Destroy();

            foreach (CombatGridTile tile in this.enemyPlatforms)
                if (tile.AssignedCharacter != null)
                    tile.AssignedCharacter.Destroy();

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
                this.playerCombatPrefabsDict.Add(combatPrefab.name, combatPrefab);

            foreach (CharacterCombat combatPrefab in this.enemyCombatPrefabs)
                this.enemyCombatPrefabsDict.Add(combatPrefab.name, combatPrefab);
        }
        
        private CharacterCombat InstantiatePlayerCombat(CharacterStats playerStats)
        {
            CharacterCombat player = Instantiate(this.playerCombatPrefabsDict[playerStats.PrefabID]);
            player.Stats = playerStats;
            return player;
        }

        private CharacterCombat InstantiateEnemyCombat(CharacterStats enemyStats)
        {
            CharacterCombat enemy = Instantiate(this.enemyCombatPrefabsDict[enemyStats.PrefabID]);
            enemyStats.ResetHealth();
            enemy.Stats = enemyStats;
            return enemy;
        }

        private bool IsPlayerGridFull()
        {
            return this.playersCount >= this.playerPlatforms.Length;
        }

        private bool IsEnemyGridFull()
        {
            return this.enemiesCount >= this.enemyPlatforms.Length;
        }

        private bool DictsLoaded() => this.playerCombatPrefabsDict.Count > 0 && this.enemyCombatPrefabsDict.Count > 0;

        private List<CharacterCombat> CreateCharacterList(CombatGridTile[] tileArray, Predicate<CombatGridTile> predicate) => FillCharacterList(tileArray, predicate, null);

        private List<CharacterCombat> FillCharacterList(CombatGridTile[] tileArray, Predicate<CombatGridTile> predicate, List<CharacterCombat> listToReturn)
        {
            List<CharacterCombat> list = listToReturn is not null ? listToReturn : new();

            foreach (CombatGridTile tile in tileArray)
                if (tile.AssignedCharacter && predicate(tile))
                    list.Add(tile.AssignedCharacter);

            return list;
        }

        private IEnumerable<string> GetTotalLoot(Func<CharacterStats, IEnumerable<EntityLoot>> lootCollectionDelegate)
        {
            List<string> totalLoot = new();

            foreach (CharacterStats character in this.defeatedEnemies)
                foreach (EntityLoot loot in lootCollectionDelegate(character))
                    loot.RollForLoot(totalLoot);

            return totalLoot;
        }
        #endregion
    }
}
