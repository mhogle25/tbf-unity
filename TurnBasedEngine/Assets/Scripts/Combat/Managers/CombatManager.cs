using UnityEngine;
using System.Collections.Generic;
using BF2D.Game;
using DataStructures.PriorityQueue;
using System;
using BF2D.UI;
using BF2D.Game.Actions;
using BF2D.Enums;

namespace BF2D.Combat
{
    public class CombatManager : MonoBehaviour
    {
        public class InitializeInfo
        {
            public List<CharacterStats> players = new();
            public List<CharacterStats> enemies = new();
        }

        [SerializeField] private OptionsGridControl mainMenu = null;
        [SerializeField] private DialogTextboxControl textboxControl = null;
        [SerializeField] private CombatGrid combatGrid = null;
        [SerializeField] private CharacterTargeter characterTargeter = null;
        [Header("Prefabs")]
        [SerializeField] private List<CharacterCombat> playerCombatPrefabs = new();
        [SerializeField] private List<CharacterCombat> enemyCombatPrefabs = new();

        private readonly Dictionary<string, CharacterCombat> playerCombatDict = new();
        private readonly Dictionary<string, CharacterCombat> enemyCombatDict = new();

        public List<CharacterCombat> Players { get { return this.players; } }
        private List<CharacterCombat> players = new();

        public List<CharacterCombat> Enemies { get { return this.enemies; } }
        private List<CharacterCombat> enemies = new();

        public List<CharacterCombat> Characters
        {
            get
            {
                List<CharacterCombat> characterCombats = new();

                foreach (CharacterCombat playerCombat in this.players)
                {
                    characterCombats.Add(playerCombat);
                }

                foreach (CharacterCombat enemyCombat in this.enemies)
                {
                    characterCombats.Add(enemyCombat);
                }

                return characterCombats;
            }
        }

        public CharacterCombat CurrentCharacter { get { return this.currentCharacter; } }
        private CharacterCombat currentCharacter = null;

        private readonly PriorityQueue<CharacterCombat, uint> characterQueue = new(0);

        private readonly Queue<CombatAction> combatActionQueue = new();

        private Action listener = null;

        public static CombatManager Instance { get { return CombatManager.instance; } }
        private static CombatManager instance = null;

        private void Awake()
        {
            SingletonSetup();
            this.listener = CombatInfoListen;
        }

        private void Update()
        {
            this.listener?.Invoke();
        }


        #region Public Utilities
        public void ExecuteItem(ItemInfo itemInfo)
        {
            Item item = itemInfo.Get();
            foreach (CharacterStatsAction statsAction in item.OnUse.StatsActions)
            {
                this.characterTargeter.Enqueue(statsAction);
            }
            this.characterTargeter.Run((combatActions) =>
            {
                this.currentCharacter.RunItemAction(new ItemCombatAction
                {
                    CombatActions = combatActions,
                    Item = item
                });
            });
        }
        #endregion

        #region Listeners
        private void CombatInfoListen()
        {
            InitializeInfo combatInfo = GameInfo.Instance.UnstageCombatInfo();
            if (combatInfo is null)
                return;

            foreach (CharacterStats stats in combatInfo.enemies)
            {
                Debug.Log(stats.Name);
            }

            Initialize(combatInfo.players, combatInfo.enemies);
            this.listener = null;
        }
        #endregion

        #region Private Methods
        private void Initialize(List<CharacterStats> players, List<CharacterStats> enemies)
        {
            LoadCharacterPrefabs();

            foreach(CharacterStats playerStats in players)
            {
                this.players.Add(InstantiatePlayerCombat(playerStats));
            }

            foreach (CharacterStats enemyStats in enemies)
            {
                this.enemies.Add(InstantiateEnemyCombat(enemyStats));
            }

            FillCharacterQueue();

            UIControlsManager.Instance.TakeControl(this.textboxControl);
            this.textboxControl.Textbox.Message("Enemies approach.", () => BeginCombat());
        }

        private void BeginCombat()
        {
            this.currentCharacter = PopCharacterQueue();
            UIControlsManager.Instance.TakeControl(this.mainMenu);
        }

        private void LoadCharacterPrefabs()
        {
            foreach (CharacterCombat combatPrefab in this.playerCombatPrefabs)
            {
                this.playerCombatDict.Add(combatPrefab.name, combatPrefab);
            }

            foreach (CharacterCombat combatPrefab in this.enemyCombatPrefabs)
            {
                this.enemyCombatDict.Add(combatPrefab.name, combatPrefab);
            }
        }

        private CharacterCombat InstantiatePlayerCombat(CharacterStats playerStats)
        {
            if (playerStats is null)
            {
                Debug.LogWarning("[CombatManager] Tried to instantiate a PlayerCombat but the stats given were null");
                return null;
            }

            if (!this.playerCombatDict.ContainsKey(playerStats.ID))
            {
                Debug.LogError($"[CombatManager] Tried to instantiate the PlayerCombat at ID {playerStats.ID} but the ID could not be found");
                return null;
            }

            return InstantiateCharacterCombat(playerStats);
        }

        private CharacterCombat InstantiateEnemyCombat(CharacterStats enemyStats)
        {
            if (enemyStats is null)
            {
                Debug.LogWarning("[CombatManager] Tried to instantiate a EnemyCombat but the stats given were null");
                return null;
            }

            if (!this.enemyCombatDict.ContainsKey(enemyStats.ID))
            {
                Debug.LogError($"[CombatManager] Tried to instantiate the EnemyCombat at ID {enemyStats.ID} but the ID could not be found");
                return null;
            }

            return InstantiateCharacterCombat(enemyStats);
        }

        private CharacterCombat InstantiateCharacterCombat(CharacterStats playerStats)
        {

            CharacterCombat playerCombat = Instantiate(this.playerCombatDict[playerStats.ID]);
            playerCombat.Stats = playerStats;
            playerCombat.TextboxControl = this.textboxControl;
            this.combatGrid.UpdatePlayerPosition(playerCombat, playerCombat.Stats.GridPosition);
            return playerCombat;
        }

        private void FillCharacterQueue()
        {
            foreach(CharacterCombat combatControl in this.players)
            {
                this.characterQueue.Insert(combatControl, combatControl.Stats.Speed);
            }

            foreach (CharacterCombat combatControl in this.enemies)
            {
                this.characterQueue.Insert(combatControl, combatControl.Stats.Speed);
            }
        }

        private CharacterCombat PopCharacterQueue()
        {
            if (this.characterQueue.Top() is null)
                return null;
            return this.characterQueue.Pop();
        }

        private void SingletonSetup()
        {
            //Setup of Monobehaviour Singleton
            if (CombatManager.instance is not null)
            {
                if (CombatManager.instance != this)
                {
                    Destroy(CombatManager.instance.gameObject);
                }
            }

            CombatManager.instance = this;
        }

        #endregion
    }
}
