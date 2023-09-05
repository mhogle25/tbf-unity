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
            if (!CombatCtx.One.CombatIsOver())
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
            if (!CombatCtx.One.CombatIsOver())
            {
                Debug.LogWarning($"[CombatGrid:GetTotalCurrencyLoot] Tried to get the total {Strings.Game.CURRENCY} loot from the fight but combat wasn't over.");
                return 0;
            }

            int value = 0;
            foreach (CharacterStats character in this.defeatedEnemies)
                value += character.Loot.CurrencyLoot;

            value += this.activeEncounter.Loot.CurrencyLoot;
            return value;
        }

        public int GetTotalEtherLoot()
        {
            if (!CombatCtx.One.CombatIsOver())
            {
                Debug.LogWarning($"[CombatGrid:GetTotalEtherLoot] Tried to get the total {Strings.Game.ETHER} loot from the fight but combat wasn't over.");
                return 0;
            }

            int value = 0;
            foreach (CharacterStats character in this.defeatedEnemies)
                value += character.Loot.EtherLoot;

            value += this.activeEncounter.Loot.EtherLoot;
            return value;
        }

        public IEnumerable<string> GetTotalItemLoot()
        {
            if (!CombatCtx.One.CombatIsOver())
            {
                Debug.LogWarning($"[CombatGrid:GetTotalItemLoot] Tried to get the total item loot from the fight but combat wasn't over.");
                return new List<string>();
            }

            return GetTotalLoot(this.activeEncounter.Loot.ItemLoot, character => character.Loot.ItemLoot);
        }

        public IEnumerable<string> GetTotalEquipmentLoot()
        {
            if (!CombatCtx.One.CombatIsOver())
            {
                Debug.LogWarning($"[CombatGrid:GetTotalEquipmentLoot] Tried to get the total equipment loot from the fight but combat wasn't over.");
                return new List<string>();
            }

            return GetTotalLoot(this.activeEncounter.Loot.EquipmentLoot, character => character.Loot.EquipmentLoot);
        }

        public IEnumerable<string> GetTotalGemLoot()
        {
            if (!CombatCtx.One.CombatIsOver())
            {
                Debug.LogWarning($"[CombatGrid:GetTotalGemLoot] Tried to get the total gem loot from the fight but combat wasn't over.");
                return new List<string>();
            }

            return GetTotalLoot(this.activeEncounter.Loot.GemLoot, character => character.Loot.GemLoot);
        }

        public IEnumerable<string> GetTotalRuneLoot()
        {
            if (!CombatCtx.One.CombatIsOver())
            {
                Debug.LogWarning($"[CombatGrid:GetTotalRuneLoot] Tried to get the total rune loot from the fight but combat wasn't over.");
                return new List<string>();
            }

            return GetTotalLoot(this.activeEncounter.Loot.RuneLoot, character => character.Loot.RuneLoot);
        }

        public void Setup(Party party, Encounter encounter)
        {
            this.activeParty = party;
            this.activeEncounter = encounter;

            this.defeatedEnemies.Clear();

            if (!DictsLoaded())
                LoadCharacterPrefabs();

            SetupCharacters(party, this.playerPlatforms, ref this.playersCount, InstantiatePlayerCombat);
            SetupCharacters(encounter, this.enemyPlatforms, ref this.enemiesCount, InstantiateEnemyCombat);

            SpeedSort();
        }

        public void PassTurn()
        {
            List<CharacterCombat> toBeDestroyed = new();

            this.characterQueue.RemoveAll((character) =>
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

        public void MovePlayer(int sourceIndex, int destinationIndex) => MoveCharacter(sourceIndex, destinationIndex, this.playerPlatforms);

        public void MoveEnemy(int sourceIndex, int destinationIndex) => MoveCharacter(sourceIndex, destinationIndex, this.enemyPlatforms);

        public void MakeEnemyLeader(CharacterCombat newLeader)
        {
            if (this.activeEncounter is null)
            {
                Debug.LogError("[CombatGrid:MakeEnemyLeader] An encounter must be active to change the encounter's leader.");
                return;
            }

            newLeader.MakeLeader(this.activeEncounter);
        }

        public void MakePlayerLeader(CharacterCombat newLeader)
        {
            if (this.activeParty is null)
            {
                Debug.LogError("[CombatGrid:MakePlayerLeader] An party must be active to change the party leader.");
                return;
            }

            newLeader.MakeLeader(this.activeParty);
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
            return player;
        }

        private CharacterCombat InstantiateEnemyCombat(ICharacterInfo enemyInfo)
        {
            CharacterCombat enemy = Instantiate(this.enemyCombatPrefabsDict[enemyInfo.Stats.PrefabID]);
            enemyInfo.Stats.ResetHealth();
            enemy.CharacterInfo = enemyInfo;
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
            List<string> totalLoot = new();

            foreach (CharacterStats character in this.defeatedEnemies)
                foreach (UtilityEntityLoot loot in lootCollectionDelegate(character))
                    loot.RollForLoot(totalLoot);

            foreach (UtilityEntityLoot loot in collection)
                loot.RollForLoot(totalLoot);

            return totalLoot;
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

                CharacterCombat playerCombat = creator(info);
                playerCombat.SetPosition(platforms[info.Position], info.Position);
                this.characterQueue.Add(playerCombat);
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
