using System;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D.Game.Combat
{
    public class CombatGrid : MonoBehaviour
    {
        [Header("Platforms")]
        [SerializeField] private CombatGridTile[] playerPlatforms = new CombatGridTile[Numbers.maxPartySize];
        [SerializeField] private CombatGridTile[] enemyPlatforms = new CombatGridTile[Numbers.maxPartySize];
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

        private Party activeParty = null;
        private Encounter activeEncounter = null;

        private int currentCharacterIndex = 0;

        #region Getter Setters
        public CharacterCombat[] ActivePlayers => CreateCharacterList(this.playerPlatforms, tile => !tile.AssignedCharacter.Stats.Dead).ToArray();
        public CharacterCombat[] ActiveEnemies => CreateCharacterList(this.enemyPlatforms, tile => !tile.AssignedCharacter.Stats.Dead).ToArray();
        public CharacterCombat[] ActiveCharacters
        {
            get
            {
                List<CharacterCombat> list = new();
                FillCharacterList(this.playerPlatforms, tile => !tile.AssignedCharacter.Stats.Dead, list);
                FillCharacterList(this.enemyPlatforms, tile => !tile.AssignedCharacter.Stats.Dead, list);
                return list.ToArray();
            }
        }

        public CharacterCombat[] AllPlayers => CreateCharacterList(this.playerPlatforms, tile => true).ToArray();
        public CharacterCombat[] AllEnemies => CreateCharacterList(this.enemyPlatforms, tile => true).ToArray();
        public CharacterCombat[] AllCharacters
        {
            get
            {
                List<CharacterCombat> list = new();
                FillCharacterList(this.playerPlatforms, tile => true, list);
                FillCharacterList(this.enemyPlatforms, tile => true, list);
                return list.ToArray();
            }
        }

        public CharacterCombat CurrentCharacter => this.characterQueue[this.currentCharacterIndex];
        #endregion

        #region Public Utilities

        public long GetTotalExperience()
        {
            long value = 0;
            foreach (CharacterStats character in this.defeatedEnemies)
                value += character.CurrentJob.ExperienceAward;
            
            return value;
        }

        public int GetTotalCurrencyLoot()
        {
            int value = 0;
            foreach (CharacterStats character in this.defeatedEnemies)
                value += character.Loot.CurrencyLoot.Calculate(this.activeParty.Leader.Stats);

            value += this.activeEncounter.Loot.CurrencyLoot.Calculate(this.activeParty.Leader.Stats);
            return value;
        }

        public int GetTotalEtherLoot()
        {
            int value = 0;
            foreach (CharacterStats character in this.defeatedEnemies)
                value += character.Loot.EtherLoot.Calculate(this.activeParty.Leader.Stats);

            value += this.activeEncounter.Loot.EtherLoot.Calculate(this.activeParty.Leader.Stats);
            return value;
        }

        public IEnumerable<string> GetTotalItemLoot() =>
            GetTotalLoot(this.activeEncounter.Loot.ItemLoot, character => character.Loot.ItemLoot);

        public IEnumerable<string> GetTotalEquipmentLoot() =>
            GetTotalLoot(this.activeEncounter.Loot.EquipmentLoot, character => character.Loot.EquipmentLoot);

        public IEnumerable<string> GetTotalGemLoot() =>
            GetTotalLoot(this.activeEncounter.Loot.GemLoot, character => character.Loot.GemLoot);

        public IEnumerable<string> GetTotalRuneLoot() => 
            GetTotalLoot(this.activeEncounter.Loot.RuneLoot, character => character.Loot.RuneLoot);
        
        /// <summary>
        /// Sets up the combat grid by creating character controllers, assigning them to tiles in the combat grid,
        /// and doubly-linking a reference to itself on the associated character's info object
        /// </summary>
        /// <param name="party">The current party. Contains information on players and their global stats.</param>
        /// <param name="encounter">The current encounter. Contains information on enemies, combat loot, etc.</param>
        public void Setup(Party party, Encounter encounter)
        {
            this.activeParty = party;
            this.activeEncounter = encounter;

            this.defeatedEnemies.Clear();

            if (!DictsLoaded())
                LoadCharacterPrefabs();

            SetupCharacters(party, this.playerPlatforms, ref this.playersCount, InstantiatePlayerCombat);
            SetupCharacters(encounter, this.enemyPlatforms, ref this.enemiesCount, InstantiateEnemyCombat);
        }

        public void PassTurn()
        {
            List<CharacterCombat> toBeDestroyed = new();

            this.characterQueue.RemoveAll(character =>
            {
                if (character.Stats.Dead)
                {
                    if (character.IsEnemy)
                    {
                        this.defeatedEnemies.Add(character.Stats);
                        this.enemiesCount--;
                    }

                    if (character.IsPlayer)
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

        public void GridReset()
        {
            foreach (CombatGridTile tile in this.playerPlatforms)
                if (tile.AssignedCharacter)
                    tile.AssignedCharacter.Destroy();

            foreach (CombatGridTile tile in this.enemyPlatforms)
                if (tile.AssignedCharacter)
                    tile.AssignedCharacter.Destroy();

            this.characterQueue.Clear();
            this.defeatedEnemies.Clear();
            this.currentCharacterIndex = 0;
            this.playersCount = 0;
            this.enemiesCount = 0;
        }

        public void MovePlayer(int sourceIndex, int destinationIndex) => MoveCharacter(sourceIndex, destinationIndex, this.playerPlatforms);

        public void MoveEnemy(int sourceIndex, int destinationIndex) => MoveCharacter(sourceIndex, destinationIndex, this.enemyPlatforms);

        public void MakeEnemyLeader(ICharacterInfo newLeader)
        {
            if (this.activeEncounter is null)
            {
                Debug.LogError("[CombatGrid:MakeEnemyLeader] An encounter must be active to change the encounter's leader.");
                return;
            }

            this.activeEncounter?.ChangeLeader(newLeader);
        }

        public void MakePlayerLeader(ICharacterInfo newLeader)
        {
            if (this.activeParty is null)
            {
                Debug.LogError("[CombatGrid:MakePlayerLeader] An party must be active to change the party leader.");
                return;
            }

            this.activeParty.ChangeLeader(newLeader);
        }
        #endregion

        #region Private Methods
        private void SpeedSort() => this.characterQueue.Sort((x, y) => y.Stats.Speed.CompareTo(x.Stats.Speed));

        private void LoadCharacterPrefabs()
        {
            foreach (CharacterCombat combatPrefab in this.playerCombatPrefabs)
                this.playerCombatPrefabsDict.Add(combatPrefab.name, combatPrefab);

            foreach (CharacterCombat combatPrefab in this.enemyCombatPrefabs)
                this.enemyCombatPrefabsDict.Add(combatPrefab.name, combatPrefab);
        }

        private bool DictsLoaded() => this.playerCombatPrefabsDict.Count > 0 && this.enemyCombatPrefabsDict.Count > 0;

        private CharacterCombat InstantiatePlayerCombat(ICharacterInfo playerInfo)
        {
            CharacterCombat player = Instantiate(this.playerCombatPrefabsDict[playerInfo.Stats.PrefabID]);
            player.CharacterInfo = playerInfo;
            playerInfo.CurrentController = player;
            return player;
        }

        private CharacterCombat InstantiateEnemyCombat(ICharacterInfo enemyInfo)
        {
            CharacterCombat enemy = Instantiate(this.enemyCombatPrefabsDict[enemyInfo.Stats.PrefabID]);
            enemyInfo.Stats.ResetHealth();
            enemy.CharacterInfo = enemyInfo;
            enemyInfo.CurrentController = enemy;
            return enemy;
        }

        private List<CharacterCombat> CreateCharacterList(CombatGridTile[] tileArray, Predicate<CombatGridTile> predicate) => FillCharacterList(tileArray, predicate, null);

        private List<CharacterCombat> FillCharacterList(CombatGridTile[] tileArray, Predicate<CombatGridTile> predicate, List<CharacterCombat> listToReturn)
        {
            List<CharacterCombat> list = listToReturn is not null ? listToReturn : new();

            foreach (CombatGridTile tile in tileArray)
                if (tile.AssignedCharacter && predicate(tile))
                    list.Add(tile.AssignedCharacter);

            return list;
        }

        private IEnumerable<string> GetTotalLoot(IEnumerable<UtilityEntityLoot> collection, Func<CharacterStats, IEnumerable<UtilityEntityLoot>> lootCollectionDelegate)
        {
            foreach (CharacterStats character in this.defeatedEnemies)
                foreach (UtilityEntityLoot loot in lootCollectionDelegate(character))
                    foreach (string id in loot.RollForLoot())
                        yield return id;

            foreach (UtilityEntityLoot loot in collection)
                foreach (string id in loot.RollForLoot())
                    yield return id;
        }

        private void SetupCharacters(CharacterGroup group, CombatGridTile[] platforms, ref int count, Func<ICharacterInfo, CharacterCombat> creator)
        {
            foreach (ICharacterInfo info in group.ActiveCharacters)
            {
                if (count >= platforms.Length)
                {
                    Debug.LogError("[CombatGrid:SetupCharacters] Tried to instantiate a character combat but the grid was full");
                    continue;
                }

                if (!Numbers.ValidGridIndex(info.Position))
                {
                    Debug.LogError($"[CombatGrid:SetupCharacters] Character '{info.Stats.Name}' had an invalid grid position -> {info.Position}");
                    continue;
                }

                CharacterCombat characterCombat = creator(info);
                characterCombat.SetPosition(platforms[info.Position], info.Position);
                this.characterQueue.Add(characterCombat);
                count++;
            }
        }

        private void MoveCharacter(int sourceIndex, int destinationIndex, CombatGridTile[] platforms)
        {
            if (!Numbers.ValidGridIndex(sourceIndex) || !Numbers.ValidGridIndex(destinationIndex))
            {
                Debug.LogError($"[CombatGrid:MoveCharacter] One or both of the given indexes were outside of the range of the grid -> Source: {sourceIndex}, Destination: {destinationIndex}");
                return;
            }

            CombatGridTile destination = platforms[destinationIndex];
            CharacterCombat character = platforms[sourceIndex].AssignedCharacter;
            character.SetPosition(destination, sourceIndex, destinationIndex);
        }
        #endregion
    }
}
